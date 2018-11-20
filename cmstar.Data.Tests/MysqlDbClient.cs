using System.Data.Common;
using MySql.Data.MySqlClient;

namespace cmstar.Data
{
    /// <summary>
    /// Mysql数据库访问客户端。
    /// </summary>
    public class MysqlDbClient : AbstractDbClient
    {
        /// <summary>
        /// 使用指定的数据库类型和连接字符串初始化<see cref="SqlDbClient"/>的新实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        public MysqlDbClient(string connectionString)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, "connectionString");
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 获取当前实例所使用的数据库连接字符串。
        /// </summary>
        public override string ConnectionString { get; }

        /// <summary>
        /// 获取当前实例所使用的<see cref="DbProviderFactory"/>实例。
        /// </summary>
        protected override DbProviderFactory Factory => FixedMySqlClientFactory.Instance;

        /// <summary>
        /// 在 MySql.Data 库的早期版本有重写<see cref="DbProviderFactory.CreateDataAdapter"/>，
        /// 但之后又移除了（坑……），我们需要重写此方法，否则使用后期版本的库将返回null。
        /// </summary>
        private class FixedMySqlClientFactory : DbProviderFactoryWrapper
        {
            public static readonly FixedMySqlClientFactory Instance = new FixedMySqlClientFactory();

            private FixedMySqlClientFactory() : base(MySqlClientFactory.Instance)
            {
            }

            public override DbDataAdapter CreateDataAdapter()
            {
                return new MySqlDataAdapter();
            }
        }
    }
}
