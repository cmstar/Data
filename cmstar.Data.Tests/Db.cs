using System.Data.Common;

namespace cmstar.Data
{
    using System.Data.SqlClient;
    using MySql.Data.MySqlClient;

    public static class Db
    {
        public static IDbClient Northwind
            => new DbClient("server=.;database=Northwind;trusted_connection=true;", SqlClientFactory.Instance);

        public static IDbClient MysqlDb
            => new DbClient("server=127.0.0.1;uid=test;pwd=123456;database=test", MySqlClientFactory.Instance);
    }


    public class FixedMySqlClientFactory : DbProviderFactoryWrapper
    {
        public static readonly FixedMySqlClientFactory Instance = new FixedMySqlClientFactory();

        private FixedMySqlClientFactory() : base(MySqlClientFactory.Instance) { }

        public override DbDataAdapter CreateDataAdapter()
        {
            return base.CreateDataAdapter() ?? new MySqlDataAdapter();
        }
    }
}
