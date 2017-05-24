using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// 提供<see cref="IDbClient"/>的封装。
    /// 可继承此类型并重写其中的方法以便定制<see cref="IDbClient"/>的行为。
    /// </summary>
    public class DbClientWrapper : IDbClient
    {
        private IDbClient _client;

        /// <summary>
        /// 初始化类型的新实例，并指定被封装的<see cref="IDbClient"/>实例。
        /// </summary>
        /// <param name="client">被封装的<see cref="IDbClient"/>实例。</param>
        public DbClientWrapper(IDbClient client)
        {
            Client = client;
        }

        /// <summary>
        /// 获取或设置当前实例所封装的<seealso cref="IDbClient"/>实例。
        /// </summary>
        public IDbClient Client
        {
            get
            {
                return _client;
            }
            set
            {
                ArgAssert.NotNull(value, "value");
                _client = value;
            }
        }

        public virtual object Scalar(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.Scalar(sql, parameters, commandType, timeout);
        }

        public virtual int Execute(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.Execute(sql, parameters, commandType, timeout);
        }

        public virtual void SizedExecute(int expectedSize, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            _client.SizedExecute(expectedSize, sql, parameters, commandType, timeout);
        }

        public virtual DataTable DataTable(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.DataTable(sql, parameters, commandType, timeout);
        }

        public virtual DataSet DataSet(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.DataSet(sql, parameters, commandType, timeout);
        }

        public virtual bool Exists(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.Exists(sql, parameters, commandType, timeout);
        }

        public virtual IDataRecord GetRow(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.GetRow(sql, parameters, commandType, timeout);
        }

        public object[] ItemArray(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ItemArray(sql, parameters, commandType, timeout);
        }

        public virtual T Get<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.Get(mapper, sql, parameters, commandType, timeout);
        }

        public virtual T ForceGet<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ForceGet(mapper, sql, parameters, commandType, timeout);
        }

        public virtual IList<T> List<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.List(mapper, sql, parameters, commandType, timeout);
        }

        public virtual IEnumerable<IDataRecord> Rows(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.Rows(sql, parameters, commandType, timeout);
        }

        public virtual ITransactionKeeper CreateTransaction()
        {
            return _client.CreateTransaction();
        }

        public virtual DbParameter CreateParameter()
        {
            return _client.CreateParameter();
        }

        public virtual string ConnectionString
        {
            get { return _client.ConnectionString; }
        }
    }
}
