using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace cmstar.Data
{
    /// <summary>
    /// <see cref="IDbClient"/>的基本实现。
    /// 这是一个抽象类。
    /// </summary>
    public abstract partial class AbstractDbClient : IDbClientAsync
    {
        /// <summary>
        /// 获取查询的第一行第一列的值。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>查询结果的第一行第一列的值。若查询结果行数为0，返回<c>null</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual async Task<object> ScalarAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        /// <summary>
        /// 执行非查询SQL语句，并断言所影响的行数。若影响的函数不正确，抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">非查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public virtual async Task<int> ExecuteAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        /// <summary>
        /// 执行非查询SQL语句，并断言所影响的行数。若影响的函数不正确，抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="expectedSize">被断言的影响行数。</param>
        /// <param name="sql">非查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public virtual async Task SizedExecuteAsync(int expectedSize,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var actualSize = await ExecuteAsync(sql, parameters, commandType, timeout);
            if (actualSize != expectedSize)
                throw new IncorrectResultSizeException(sql, commandType, parameters, expectedSize, actualSize);
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataTable"/>。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual async Task<DataTable> DataTableAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);

                // TODO 目前还没有找到适当的异步填充 DataTable 的方法，填充部分目前以非异步方式执行。
                // dataTable.Load(reader) 在 mysql 的部分类型字段（比如longtext）将出现异常，必须通过 mysql
                // 的 connector library 提供的 MySqlDataAdapter 填充，解决方案尚不清晰。
                return FillDataTable(cmd);
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataSet"/>。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataSet"/>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual async Task<DataSet> DataSetAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);

                // TODO 目前还没有找到适当的异步填充 DataSet 的方法，填充部分目前以非异步方式执行。
                // 主要因为 DbDataAdaper 没有提供异步的方法，而如何从 IDataReader 获得 DataSet还没搞清楚。
                return FillDataSet(cmd);
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        /// <summary>
        /// 判断给定的查询的结果是否至少包含1行。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>若查询结果至少包含1行，返回<c>true</c>；否则返回<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual async Task<bool> ExistsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await ScalarAsync(sql, parameters, commandType, timeout) != null;
        }

        /// <summary>
        /// 获取查询结果的第一行记录。
        /// 若查询命中的行数为0，返回null。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns><see cref="IDataRecord"/>的实现，包含查询的第一行记录。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        /// <remarks>
        /// 区别于<see cref="DbCommand.ExecuteReader()"/>的用法，此方法执行完毕后将并不保持数据库连接，
        /// 也不需要调用<see cref="IDisposable.Dispose"/>。
        /// </remarks>
        public virtual async Task<IDataRecord> GetRowAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await GetAsync(SingleRowKeeperMapper.Instance, sql, parameters, commandType, timeout);
        }

        /// <summary>
        /// 获取查询结果的第一行记录，以数组形式返回记录内各列的值。
        /// 数组元素顺序与列顺序一致。若查询命中的行数为0，返回null。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>包含了各列的值的数组。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual async Task<object[]> ItemArrayAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await GetAsync(ItemArrayMapper.Instance, sql, parameters, commandType, timeout);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <returns>目标类型的实例。</returns>
        public virtual async Task<T> GetAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));
            ArgAssert.NotNull(mapper, nameof(mapper));

            DbConnection connection = null;
            IDataReader reader = null;
            DbCommand cmd = null;
            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = await cmd.ExecuteReaderAsync();
                return reader.Read() ? mapper.MapRow(reader, 1) : default(T);
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// SQL命中的记录必须为1行，否则抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public virtual async Task<T> ForceGetAsync<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            DbConnection connection = null;
            IDataReader reader = null;
            DbCommand cmd = null;
            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = await cmd.ExecuteReaderAsync();

                int rowCount;
                T result;
                if (DbClientHelper.TryMapUniqueRow(reader, mapper, out result, out rowCount))
                    return result;

                throw new IncorrectResultSizeException(sql, CommandType.Text, parameters, 1, rowCount);
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象的集合。
        /// 若查询未命中纪录，返回空集（长度为0，不是null）。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>目标类型的实例的集合。若查询命中的行数为0，返回空集合。</returns>
        public virtual async Task<IList<T>> ListAsync<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));
            ArgAssert.NotNull(mapper, nameof(mapper));

            DbConnection connection = null;
            DbCommand cmd = null;
            IDataReader reader = null;
            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = await cmd.ExecuteReaderAsync();
                return MapRowsToList(reader, mapper);
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        /// <summary>
        /// 获取查询结果得行序列。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>查询结果得行序列。</returns>
        public virtual async Task<IEnumerable<IDataRecord>> RowsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbDataReader reader = null;
            DbCommand cmd = null;

            try
            {
                connection = await CreateAndOpenConnectionAsync();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = await cmd.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                cmd?.Parameters.Clear();

                //获取reader失败时，关闭连接
                if (reader == null && connection != null)
                    CloseConnection(connection);
            }

            return YieldRows(connection, reader);
        }

        /// <summary>
        /// 打开指定的数据库连接。
        /// 此方法在各<see cref="IDbClient"/>方法中的命令执行前被调用，重写此方法以控制其行为。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        protected virtual async Task OpenConnectionAsync(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
        }

        private async Task<DbConnection> CreateAndOpenConnectionAsync()
        {
            var connection = CreateConnection();
            await OpenConnectionAsync(connection);
            return connection;
        }
    }
}
