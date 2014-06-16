using cmstar.Data;
using System.Data.Common;

namespace cmstar.DalSupport.Tests
{
    /// <summary>
    /// Mysql数据库访问客户端。
    /// </summary>
    public class MysqlDbClient : AbstractDbClient
    {
        private readonly string _connectionString;

        /// <summary>
        /// 使用指定的数据库类型和连接字符串初始化<see cref="SqlDbClient"/>的新实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        public MysqlDbClient(string connectionString)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, "connectionString");
            _connectionString = connectionString;
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
            get { return MySql.Data.MySqlClient.MySqlClientFactory.Instance; }
        }
    }
}
