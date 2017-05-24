using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace cmstar.Data
{
    /// <summary>
    /// 与线程绑定有关的<see cref="ITransactionKeeper"/>的实现。
    /// 控制一个线程中获取到的实例总是同一个，使事务嵌套的情况下，内层申明的事务实际上跑在外层的实例作用域中。
    /// 类型的实例成员并不是线程安全的。 
    /// </summary>
    /// <remarks>
    /// 若<see cref="IDisposable.Dispose"/>在<see cref="ITransactionKeeper.Commit"/>之前被调用，
    /// 事务将被回滚。
    /// 注意，所有语句都将在事务内运行，包括查询。
    /// </remarks>
    public class ThreadLocalTransactionKeeper : AbstractDbClient, ITransactionKeeper
    {
        private static readonly Dictionary<int, ThreadLocalTransactionKeeper> Keepers
            = new Dictionary<int, ThreadLocalTransactionKeeper>();

        /// <summary>
        /// 获取绑定到当前线程的<see cref="ThreadLocalTransactionKeeper"/>实例。
        /// </summary>
        /// <param name="dbProviderFactory"><see cref="DbProviderFactory"/>实例。</param>
        /// <param name="connectionString">初始化数据库连接的连接字符串。</param>
        /// <returns><see cref="ThreadLocalTransactionKeeper"/>实例。</returns>
        public static ThreadLocalTransactionKeeper OpenTransaction(
            DbProviderFactory dbProviderFactory, string connectionString)
        {
            var theadId = Thread.CurrentThread.ManagedThreadId;
            ThreadLocalTransactionKeeper keeper;

            lock (Keepers)
            {
                if (Keepers.TryGetValue(theadId, out keeper))
                {
                    keeper._embeddedLevel++;
                }
                else
                {
                    keeper = new ThreadLocalTransactionKeeper(dbProviderFactory, connectionString);
                    Keepers.Add(theadId, keeper);
                }
            }

            return keeper;
        }

        private static void RemoveLocalTransactionKeeper()
        {
            lock (Keepers)
            {
                Keepers.Remove(Thread.CurrentThread.ManagedThreadId);
            }
        }

        private readonly object _syncRoot = new object();
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;

        private DbTransaction _transaction;
        private DbConnection _connection;
        private bool _transactionCompleted;
        private bool _disposed;
        private int _embeddedLevel;

        private ThreadLocalTransactionKeeper(DbProviderFactory dbProviderFactory, string connectionString)
        {
            ArgAssert.NotNull(dbProviderFactory, "dbProviderFactory");
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, "connectionString");

            _factory = dbProviderFactory;
            _connectionString = connectionString;
        }

        ~ThreadLocalTransactionKeeper()
        {
            Dispose(false);
        }

        /// <summary>
        /// 获取当前实例所使用的数据库连接字符串。
        /// </summary>
        public override string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// 获取当前实例所使用的<see cref="DbProviderFactory"/>实例。
        /// </summary>
        protected override DbProviderFactory Factory
        {
            get { return _factory; }
        }

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
                throw new InvalidOperationException("连接已关闭。");

            if (_transactionCompleted)
                throw new InvalidOperationException("事务已结束。");

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

                _disposed = true;
            }
            finally
            {
                RemoveLocalTransactionKeeper();

                //显式释放资源时，阻止GC调用Finalize方法
                if (disposing)
                    GC.SuppressFinalize(this);
            }
        }

        protected override DbConnection CreateConnection()
        {
            return LocalConnection();
        }

        protected override void OpenConnection(DbConnection connection)
        {
            lock (_syncRoot)
            {
                base.OpenConnection(connection);
            }
        }

        protected override void CloseConnection(DbConnection connection)
        {
            //连接在Dispose时关闭，此处什么也不做
        }

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
