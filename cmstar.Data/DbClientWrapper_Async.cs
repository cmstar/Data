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
        public virtual async Task<object> ScalarAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.ScalarAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<int> ExecuteAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.ExecuteAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task SizedExecuteAsync(int expectedSize, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            await _client.SizedExecuteAsync(expectedSize, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<DataTable> DataTableAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.DataTableAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<DataSet> DataSetAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.DataSetAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.ExistsAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<IDataRecord> GetRowAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.GetRowAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<object[]> ItemArrayAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.ItemArrayAsync(sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<T> GetAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.GetAsync(mapper, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<T> ForceGetAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.ForceGetAsync(mapper, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<IList<T>> ListAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.ListAsync(mapper, sql, parameters, commandType, timeout);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<IDataRecord>> RowsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await _client.RowsAsync(sql, parameters, commandType, timeout);
        }
    }
}
