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
        protected readonly IDbClient InternalClient;

        /// <summary>
        /// 初始化类型的新实例，并指定被封装的<see cref="IDbClient"/>实例。
        /// </summary>
        /// <param name="internalClient"><see cref="IDbClient"/>实例。</param>
        public DbClientWrapper(IDbClient internalClient)
        {
            ArgAssert.NotNull(internalClient, "client");
            InternalClient = internalClient;
        }

        public virtual object Scalar(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.Scalar(sql, parameters, commandType, timeOut);
        }

        public virtual int Execute(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.Execute(sql, parameters, commandType, timeOut);
        }

        public virtual void SizedExecute(int expectedSize, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            InternalClient.SizedExecute(expectedSize, sql, parameters, commandType, timeOut);
        }

        public virtual DataTable DataTable(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.DataTable(sql, parameters, commandType, timeOut);
        }

        public virtual DataSet DataSet(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.DataSet(sql, parameters, commandType, timeOut);
        }

        public virtual bool Exists(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.Exists(sql, parameters, commandType, timeOut);
        }

        public virtual IDataRecord GetRow(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.GetRow(sql, parameters, commandType, timeOut);
        }

        public virtual T Get<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.Get(mapper, sql, parameters, commandType, timeOut);
        }

        public virtual T ForceGet<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.ForceGet(mapper, sql, parameters, commandType, timeOut);
        }

        public virtual IList<T> List<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.List(mapper, sql, parameters, commandType, timeOut);
        }

        public virtual IEnumerable<IDataRecord> Rows(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return InternalClient.Rows(sql, parameters, commandType, timeOut);
        }

        public virtual ITransactionKeeper CreateTransaction()
        {
            return InternalClient.CreateTransaction();
        }

        public virtual DbParameter CreateParameter()
        {
            return InternalClient.CreateParameter();
        }

        public virtual string ConnectionString
        {
            get { return InternalClient.ConnectionString; }
        }
    }
}
