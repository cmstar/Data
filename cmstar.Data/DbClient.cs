using System;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// 默认的<see cref="IDbClient"/>实现。
    /// </summary>
    public class DbClient : AbstractDbClient
    {
        private readonly DbProviderFactory _factory;
        private readonly Func<DbProviderFactory> _factoryGetter;

        /// <summary>
        /// 使用指定的连接字符串和<see cref="DbProviderFactory"/>实例初始化<see cref="DbClient"/>的新实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="factory"><see cref="DbProviderFactory"/>实例</param>
        public DbClient(string connectionString, DbProviderFactory factory)
        {
            ArgAssert.NotNull(factory, nameof(factory));
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, nameof(connectionString));

            _factory = factory;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 使用指定的连接字符串和<see cref="DbProviderFactory"/>工厂初始化<see cref="DbClient"/>的新实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="factoryGetter">用于创建<see cref="DbProviderFactory"/>。</param>
        public DbClient(string connectionString, Func<DbProviderFactory> factoryGetter)
        {
            ArgAssert.NotNull(factoryGetter, nameof(factoryGetter));
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, nameof(connectionString));

            _factoryGetter = factoryGetter;
            ConnectionString = connectionString;
        }

        /// <inheritdoc />
        public override string ConnectionString { get; }

        /// <inheritdoc />
        protected override DbProviderFactory Factory => _factory ?? _factoryGetter();
    }
}
