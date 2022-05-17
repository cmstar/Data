using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// 提供单个线程使用的<see cref="ITransactionKeeper"/>的实现。
    /// 允许对一个实例调用<see cref="CreateTransaction"/>方法再次创建事务，事务具有递归层级计数，
    /// 具有递归层级的事务不实际执行提交或回滚，当递归层级为0时才会执行。
    /// 类型的实例成员并不是线程安全的，不要通过多线程并发访问。 
    /// </summary>
    /// <remarks>
    /// 若<see cref="IDisposable.Dispose"/>在<see cref="ITransactionKeeper.Commit"/>之前被调用，
    /// 事务将被回滚。
    /// 注意，所有语句都将在事务内运行，包括查询。
    /// </remarks>
    public sealed class ThreadLocalTransactionKeeper : AbstractDbClient, ITransactionKeeper
    {
        // 当前事务所使用的数据库连接。
        // 使用延迟加载，在第一次执行数据库操作时创建。
        private DbConnection _connection;

        // 当前事务的 DbTransaction 实例。
        // 使用延迟加载，在第一次执行数据库操作时创建。
        private DbTransaction _transaction;

        // 当前事务是否已经完结，若完结则不允许再执行数据库操作。
        // 初始值 false，事务提交或回滚后为 true。
        private bool _transactionCompleted;

        // Dispose 方法是否已经执行过，Dispose 后的对象不在允许其他操作，否则抛出 ObjectDisposedException。 
        private bool _disposed;

        // 事务的嵌套层级。
        // ITransactionKeeper 接口继承了 IDbClient，所以具有 CreateTransaction 方法。
        // 刚创建的事务潜逃层级为0，事务内再次创建事务时+1，并返回（复用）当前实例。
        private int _embeddedLevel;

        /// <summary>
        /// 创建<see cref="ThreadLocalTransactionKeeper"/>的新实例。
        /// </summary>
        /// <param name="dbProviderFactory"><see cref="DbProviderFactory"/>实例。</param>
        /// <param name="connectionString">初始化数据库连接的连接字符串。</param>
        /// <param name="commandTimeout">
        /// 指定事务内的命令的默认执行超时时间，当方法没有单独制定超时时，套用此超时值。
        /// </param>
        public ThreadLocalTransactionKeeper(
            DbProviderFactory dbProviderFactory, string connectionString, int commandTimeout)
        {
            ArgAssert.NotNull(dbProviderFactory, nameof(dbProviderFactory));
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, nameof(connectionString));

            Factory = dbProviderFactory;
            ConnectionString = connectionString;
            DefaultTimeout = commandTimeout;
        }

        /// <inheritdoc />
        ~ThreadLocalTransactionKeeper()
        {
            if (_disposed || _connection == null)
                return;

            // 参考：
            // https://msdn.microsoft.com/en-us/library/system.data.common.dbconnection.close.aspx
            // -----
            // Do not call Close or Dispose on a Connection, a DataReader, or any other managed object 
            // in the Finalize method of your class. In a finalizer, you should only release unmanaged 
            // resources that your class owns directly.
            // -----
            // Finalize 中不能调用 DbConnection.Close，否则会出现异常：
            //     InvalidOperationException: Internal .Net Framework Data Provider error 1.
            // 所以仅在从 Dispose 方法进入时关闭连接。但 DbConnection 自身的释放可能不会很及时，我们希望
            // 用户没有正确调用 Dispose 的情况下，事务仍能够尽快被释放掉，所以这里调用 Rollback 处理。
            try
            {
                // 对于未嵌套的事务执行回滚操作。
                if (!_transactionCompleted)
                {
                    Rollback();
                }
            }
            catch
            {
                // 忽略异常，否则在GC线程内抛出异常会导致整个程序崩溃。
            }
        }

        /// <summary>
        /// 获取当前实例所使用的数据库连接字符串。
        /// </summary>
        public override string ConnectionString { get; }

        /// <summary>
        /// 获取当前实例所使用的<see cref="DbProviderFactory"/>实例。
        /// </summary>
        protected override DbProviderFactory Factory { get; }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (_disposed || _connection == null)
                return;

            // 在嵌套事务申明中，并不真正处理嵌套的事务，仅递减嵌套级别。
            if (_embeddedLevel > 0)
            {
                _embeddedLevel--;
                return;
            }

            // 对于未嵌套的事务执行回滚操作。直接关闭连接，事务就被回滚了。
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }

            // 显式释放资源时，阻止 GC 调用 Finalize 方法。
            GC.SuppressFinalize(this);

            _disposed = true;
        }

        /// <inheritdoc cref="ITransactionKeeper.Commit" />
        public void Commit()
        {
            if (!CanCommitOrRollback)
                return;

            _transaction.Commit();
            _transactionCompleted = true;
        }

        /// <inheritdoc cref="ITransactionKeeper.Rollback" />
        public void Rollback()
        {
            if (!CanCommitOrRollback)
                return;

            _transaction.Rollback();
            _transactionCompleted = true;
        }

        /// <inheritdoc cref="AbstractDbClient.CreateConnection" />
        protected override DbConnection CreateConnection()
        {
            CheckStatus();

            // 整个事务使用同一个连接。
            if (_connection != null)
                return _connection;

            _connection = base.CreateConnection();
            return _connection;
        }

        /// <inheritdoc cref="AbstractDbClient.CloseConnection" />
        protected override void CloseConnection(DbConnection connection)
        {
            // 连接在 Dispose 时关闭，此处什么也不做。
        }

        /// <inheritdoc cref="AbstractDbClient.CreateCommand" />
        protected override DbCommand CreateCommand(string sql,
            DbConnection connection, IEnumerable<DbParameter> parameters,
            CommandType commandType, int timeout)
        {
            var cmd = base.CreateCommand(sql, connection, parameters, commandType, timeout);

            // 延迟创建事务。
            if (_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }

            // 将创建出来的 DbCommand 加入当前的事务中。
            cmd.Transaction = _transaction;

            return cmd;
        }

        /// <inheritdoc  cref="AbstractDbClient.CreateTransaction" />
        public override ITransactionKeeper CreateTransaction()
        {
            CheckStatus();

            // 在事务内再次创建事务时，复用当前实例，仅增加事务嵌套层级计数。
            _embeddedLevel++;
            return this;
        }

        private void CheckStatus()
        {
            if (_transactionCompleted)
                throw new InvalidOperationException("The transaction was finished.");

            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        // 检查当前实例的状态，在状态无效时抛出异常。若当前事务能够被提交或回滚，返回 true。
        private bool CanCommitOrRollback
        {
            get
            {
                // 事务没有被创建出来则不需要提交或回滚。
                if (_connection == null || _transaction == null)
                    return false;

                // 嵌套的事务中，实际的回滚与提交交给最外层处理，内层仅减少嵌套级别（由 Dispose 方法执行）。
                if (_embeddedLevel != 0)
                    return false;

                var state = _connection.State;
                if (state == ConnectionState.Closed || state == ConnectionState.Broken)
                    throw new InvalidOperationException("The database connction was closed.");

                CheckStatus();
                return true;
            }
        }
    }
}
