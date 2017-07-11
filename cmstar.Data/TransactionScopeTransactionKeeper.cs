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
        private readonly TransactionScope _tran;
        private bool _transactionCompleted;
        private bool _disposed;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="dbProviderFactory"><see cref="DbProviderFactory"/>的实例。</param>
        /// <param name="connectionString">指定数据库的连接字符串。</param>
        public TransactionScopeTransactionKeeper(DbProviderFactory dbProviderFactory, string connectionString)
        {
            ArgAssert.NotNull(dbProviderFactory, "dbProviderFactory");
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, "connectionString");

            Factory = dbProviderFactory;
            ConnectionString = connectionString;

            //鉴于应用场景，直接开启事务控制（不过数据库连接还没初始化）
            _tran = new TransactionScope();
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

        /// <inheritdoc cref="IDbClient.CreateTransaction"/>
        public override ITransactionKeeper CreateTransaction()
        {
            ValidateStatus();
            return this;
        }

        /// <inheritdoc cref="AbstractDbClient.CreateConnection"/>
        protected override DbConnection CreateConnection()
        {
            ValidateStatus();
            return base.CreateConnection();
        }

        private void ValidateStatus()
        {
            if (_transactionCompleted)
                throw new InvalidOperationException("The transaction was finished.");

            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
