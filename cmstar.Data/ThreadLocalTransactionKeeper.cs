using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace cmstar.Data
{
    /// <summary>
    /// 与线程绑定有关的<see cref="ITransactionKeeper"/>的实现。
    /// 控制一个线程中获取到的实例总是同一个，使事务嵌套的情况下，内层申明的事务实际上跑在外层的实例作用域中。
    /// 嵌套事务绑定在当前的代码执行路径（CallContext）上，若将同一实例传递到在多个不同执行路径的线程内，每个
    /// 线程中调用<see cref="OpenTransaction"/>方法，都将开启一个独立的新事务。
    /// 类型的实例成员并不是线程安全的。 
    /// </summary>
    /// <remarks>
    /// 若<see cref="IDisposable.Dispose"/>在<see cref="ITransactionKeeper.Commit"/>之前被调用，
    /// 事务将被回滚。
    /// 注意，所有语句都将在事务内运行，包括查询。
    /// </remarks>
    public class ThreadLocalTransactionKeeper : AbstractDbClient, ITransactionKeeper
    {
        private const string CallContextName = "CORE_DATA_TRANSACTION_THREADLOCAL";
        private static readonly object SyncBlock = new object();

        /// <summary>
        /// 获取绑定到当前线程的<see cref="ThreadLocalTransactionKeeper"/>实例。
        /// </summary>
        /// <param name="dbProviderFactory"><see cref="DbProviderFactory"/>实例。</param>
        /// <param name="connectionString">初始化数据库连接的连接字符串。</param>
        /// <returns><see cref="ThreadLocalTransactionKeeper"/>实例。</returns>
        public static ThreadLocalTransactionKeeper OpenTransaction(
            DbProviderFactory dbProviderFactory, string connectionString)
        {
            ThreadLocalTransactionKeeper keeper;

            // 为了支持事务嵌套（一个事务上再开一个事务），在同一个线程中每次访问 OpenTransaction 都应该获得
            // 同一个事务实例，并记录嵌套级别；Commit 方法仅当嵌套级别为0时才真正提交。
            // 这个事务实例一般是绑定在当前执行线程上的，但在调用异步（async）代码后，线程可能切换，为了支持
            // 异步调用下的正常访问，这里通过 CallContext.LogicalGetData/LogicalSetData 设置与找回事务实例。

            lock (SyncBlock)
            {
                keeper = (ThreadLocalTransactionKeeper)CallContext.LogicalGetData(CallContextName);

                // 同一个上下文（CallContext）的代码可能重复的开启关闭多次事务，每次提交或回滚后，再次开启新
                // 事务时，应创建新的实例，避免拿到之前已经 Dispose 的实例。
                // 虽然 Dispose 时会将 CallContext 里的引用清理掉，但每个方法的文档都说*可能会*抛出异常，为了
                // 防止没有成功清理，这里也判断一下 _disposed 字段的值。
                if (keeper == null || keeper._disposed)
                {
                    keeper = new ThreadLocalTransactionKeeper(dbProviderFactory, connectionString);
                    CallContext.LogicalSetData(CallContextName, keeper);
                }
                else
                {
                    // 增加事务嵌套层级计数。
                    keeper._embeddedLevel++;
                }
            }

            return keeper;
        }

        private readonly object _syncRoot = new object();

        private DbTransaction _transaction;
        private DbConnection _connection;
        private bool _transactionCompleted;
        private bool _disposed; // Dispose 方法是否已经执行过
        private int _embeddedLevel; // 事务的嵌套层级，刚创建的事务值为0，事务内再创建则+1

        private ThreadLocalTransactionKeeper(DbProviderFactory dbProviderFactory, string connectionString)
        {
            ArgAssert.NotNull(dbProviderFactory, nameof(dbProviderFactory));
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, nameof(connectionString));

            Factory = dbProviderFactory;
            ConnectionString = connectionString;
        }

        /// <inheritdoc />
        ~ThreadLocalTransactionKeeper()
        {
            Dispose(false);
        }

        /// <summary>
        /// 获取当前实例所使用的数据库连接字符串。
        /// </summary>
        public override string ConnectionString { get; }

        /// <summary>
        /// 获取当前实例所使用的<see cref="DbProviderFactory"/>实例。
        /// </summary>
        protected override DbProviderFactory Factory { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_disposed)
                return;

            Dispose(true);
        }

        void ITransactionKeeper.Begin()
        {
            if (!LocalConnectionInitialized())
                return;

            ValidateStatus();
        }

        /// <summary>
        /// 提交事务。
        /// </summary>
        public void Commit()
        {
            if (!LocalConnectionInitialized())
                return;

            ValidateStatus();

            if (_embeddedLevel == 0)
            {
                LocalTransaction().Commit();
                _transactionCompleted = true;
            }
        }

        /// <summary>
        /// 回滚事务。
        /// </summary>
        public void Rollback()
        {
            if (!LocalConnectionInitialized())
                return;

            ValidateStatus();

            if (_embeddedLevel == 0)
            {
                LocalTransaction().Rollback();
                _transactionCompleted = true;
            }
        }

        private void ValidateStatus()
        {
            if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
                throw new InvalidOperationException("The database connction was closed.");

            if (_transactionCompleted)
                throw new InvalidOperationException("The transaction was finished.");

            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private void Dispose(bool disposing)
        {
            //在嵌套事务申明中，并不真正处理嵌套的事务，仅递减嵌套级别
            if (_embeddedLevel > 0)
            {
                _embeddedLevel--;
                return;
            }

            //对于未嵌套的事务执行真实的资源释放操作
            try
            {
                //若由Finalize方法调用，且此时刚好处于整个进程的结束期间，
                //许多依赖资源可能已被回收，可导致无法显式地回滚事务
                //故仅在显式释放资源时使用显式的事务回滚
                if (disposing && !_transactionCompleted)
                    Rollback();

                //关闭数据库连接，若此时事务未完成，则会被数据库回滚
                if (LocalConnectionInitialized())
                    _connection.Dispose();
            }
            finally
            {
                _disposed = true;

                // 用完后，需要从 CallContext 里清理掉当前对象的引用，否则引用会被一直保持着。
                CallContext.LogicalSetData(CallContextName, null);

                //显式释放资源时，阻止GC调用Finalize方法
                if (disposing)
                    GC.SuppressFinalize(this);
            }
        }

        /// <inheritdoc cref="AbstractDbClient.CreateConnection" />
        protected override DbConnection CreateConnection()
        {
            return LocalConnection();
        }

        /// <inheritdoc cref="AbstractDbClient.OpenConnection" />
        protected override void OpenConnection(DbConnection connection)
        {
            lock (_syncRoot)
            {
                base.OpenConnection(connection);
            }
        }

        /// <inheritdoc cref="AbstractDbClient.CloseConnection" />
        protected override void CloseConnection(DbConnection connection)
        {
            //连接在Dispose时关闭，此处什么也不做
        }

        /// <inheritdoc cref="AbstractDbClient.CreateCommand" />
        protected override DbCommand CreateCommand(string sql,
            DbConnection connection, IEnumerable<DbParameter> parameters,
            CommandType commandType, int timeout)
        {
            var cmd = base.CreateCommand(sql, connection, parameters, commandType, timeout);

            //将DbCommand并入本地事务
            cmd.Transaction = LocalTransaction();

            return cmd;
        }

        private DbConnection LocalConnection()
        {
            if (_connection == null)
            {
                lock (_syncRoot)
                {
                    Thread.MemoryBarrier();

                    if (_connection == null)
                        _connection = base.CreateConnection();
                }
            }
            return _connection;
        }

        private DbTransaction LocalTransaction()
        {
            if (_transaction == null)
            {
                lock (_syncRoot)
                {
                    Thread.MemoryBarrier();
                    if (_transaction == null)
                        _transaction = LocalConnection().BeginTransaction();
                }
            }
            return _transaction;
        }

        private bool LocalConnectionInitialized()
        {
            return _connection != null;
        }
    }
}
