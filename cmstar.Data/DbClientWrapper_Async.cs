using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace cmstar.Data
{
    // DbClientWrapper 的异步方法
    public partial class DbClientWrapper
    {
        /// <inheritdoc />
        public virtual Task<object> ScalarAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ScalarAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<int> ExecuteAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ExecuteAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task SizedExecuteAsync(int expectedSize, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.SizedExecuteAsync(expectedSize, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<DataTable> DataTableAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.DataTableAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<DataSet> DataSetAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.DataSetAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<bool> ExistsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ExistsAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<IDataRecord> GetRowAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.GetRowAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<object[]> ItemArrayAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ItemArrayAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<T> GetAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.GetAsync(mapper, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<T> ForceGetAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ForceGetAsync(mapper, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<IList<T>> ListAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.ListAsync(mapper, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<IDataRecord>> RowsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return _client.RowsAsync(sql, parameters, commandType, timeout);
        }
    }
}
