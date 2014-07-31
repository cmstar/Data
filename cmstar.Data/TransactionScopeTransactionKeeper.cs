using System;
using System.Data.Common;
using System.Transactions;

namespace cmstar.Data
{
    /// <summary>
    /// 使用MSDTC进行的分布式事务控制。
    /// </summary>
    public class TransactionScopeTransactionKeeper : AbstractDbClient, ITransactionKeeper
    {
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;
        private readonly TransactionScope _tran;
        private bool _transactionCompleted;
        private bool _disposed;

        public TransactionScopeTransactionKeeper(DbProviderFactory dbProviderFactory, string connectionString)
        {
            ArgAssert.NotNull(dbProviderFactory, "dbProviderFactory");
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, "connectionString");

            _factory = dbProviderFactory;
            _connectionString = connectionString;

            //鉴于应用场景，直接开启事务控制（不过数据库连接还没初始化）
            _tran = new TransactionScope();
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
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _tran.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// 开启一个事务。
        /// </summary>
        public void Begin()
        {
            ValidateStatus();
        }

        /// <summary>
        /// 提交事务。
        /// </summary>
        public void Commit()
        {
            ValidateStatus();

            _tran.Complete();
            _transactionCompleted = true;
        }

        /// <summary>
        /// 回滚事务。
        /// </summary>
        public void Rollback()
        {
            Dispose();
        }

        public override ITransactionKeeper CreateTransaction()
        {
            ValidateStatus();
            return this;
        }

        protected override DbConnection CreateConnection()
        {
            ValidateStatus();
            return base.CreateConnection();
        }

        private void ValidateStatus()
        {
            if (_transactionCompleted)
                throw new InvalidOperationException("事务已结束。");

            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
