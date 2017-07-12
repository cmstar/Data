using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// <see cref="IDbClient"/>的基本实现。
    /// 这是一个抽象类。
    /// </summary>
#if NET35
    public abstract class AbstractDbClient : IDbClient
#else
    public abstract partial class AbstractDbClient
#endif
    {
        /// <summary>
        /// 获取或设置默认的命令执行超时时间。当访问数据库的方法没有指定命令执行的超时时间（即
        /// <see cref="DbCommand.CommandTimeout"/>）时，使用此超时时间。各方法通常有 timeout 参数用于指定超时
        /// 时间，当值为0时即套用此属性的值作为超时时间。
        /// 单位为秒，初始值为0（不限制）。
        /// </summary>
        public virtual int DefaultTimeout { get; set; } = 0;

        /// <summary>
        /// 获取当前实例所使用的数据库连接字符串。
        /// </summary>
        public abstract string ConnectionString { get; }

        /// <summary>
        /// 获取当前实例所使用的<see cref="DbProviderFactory"/>实例。
        /// </summary>
        protected abstract DbProviderFactory Factory { get; }

        /// <summary>
        /// 获取查询的第一行第一列的值。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>查询结果的第一行第一列的值。若查询结果行数为0，返回<c>null</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual object Scalar(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return cmd.ExecuteScalar();
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
        /// </summary>
        /// <param name="sql">非查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public virtual int Execute(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                return cmd.ExecuteNonQuery();
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
        /// </summary>
        /// <param name="expectedSize">被断言的影响行数。</param>
        /// <param name="sql">非查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public virtual void SizedExecute(int expectedSize,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var actualSize = Execute(sql, parameters, commandType, timeout);
            if (actualSize != expectedSize)
                throw new IncorrectResultSizeException(sql, commandType, parameters, expectedSize, actualSize);
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataTable"/>。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual DataTable DataTable(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
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
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataSet"/>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual DataSet DataSet(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
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
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>若查询结果至少包含1行，返回<c>true</c>；否则返回<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual bool Exists(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return Scalar(sql, parameters, commandType, timeout) != null;
        }

        /// <summary>
        /// 获取查询结果的第一行记录。
        /// 若查询命中的行数为0，返回null。
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
        public virtual IDataRecord GetRow(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return Get(SingleRowKeeperMapper.Instance, sql, parameters, commandType, timeout);
        }

        /// <summary>
        /// 获取查询结果的第一行记录，以数组形式返回记录内各列的值。
        /// 数组元素顺序与列顺序一致。若查询命中的行数为0，返回null。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>包含了各列的值的数组。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public virtual object[] ItemArray(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return Get(ItemArrayMapper.Instance, sql, parameters, commandType, timeout);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <returns>目标类型的实例。</returns>
        public virtual T Get<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));
            ArgAssert.NotNull(mapper, nameof(mapper));

            DbConnection connection = null;
            IDataReader reader = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = cmd.ExecuteReader();
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
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public virtual T ForceGet<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));
            ArgAssert.NotNull(mapper, nameof(mapper));

            DbConnection connection = null;
            IDataReader reader = null;
            DbCommand cmd = null;
            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = cmd.ExecuteReader();

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
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>目标类型的实例的集合。若查询命中的行数为0，返回空集合。</returns>
        public virtual IList<T> List<T>(IMapper<T> mapper,
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
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = cmd.ExecuteReader();
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
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns>查询结果得行序列。</returns>
        public virtual IEnumerable<IDataRecord> Rows(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, nameof(sql));

            DbConnection connection = null;
            DbDataReader reader = null;
            DbCommand cmd = null;

            try
            {
                connection = CreateAndOpenConnection();
                cmd = CreateCommand(sql, connection, parameters, commandType, timeout);
                reader = cmd.ExecuteReader();
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
        /// 创建事务容器。
        /// </summary>
        /// <returns><see cref="ITransactionKeeper"/>。</returns>
        public virtual ITransactionKeeper CreateTransaction()
        {
            return ThreadLocalTransactionKeeper.OpenTransaction(Factory, ConnectionString, DefaultTimeout);
        }

        /// <summary>
        /// 创建一个新的SQL参数实例。
        /// </summary>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public DbParameter CreateParameter()
        {
            return Factory.CreateParameter();
        }

        /// <summary>
        /// 创建数据库连接的实例。
        /// 在各<see cref="IDbClient"/>方法中使用此方法获取连接的实例。
        /// 重写此方法以控制连接创建的行为。
        /// </summary>
        /// <returns>数据库连接的实例。</returns>
        protected virtual DbConnection CreateConnection()
        {
            var connection = Factory.CreateConnection();

            if (connection == null)
                throw new NotSupportedException("获取数据库连接失败。");

            connection.ConnectionString = ConnectionString;
            return connection;
        }

        /// <summary>
        /// 从指定的数据库连接上创建<see cref="DbCommand"/>对象。
        /// 在各<see cref="IDbClient"/>方法中使用此方法获取<see cref="DbCommand"/>对象。
        /// </summary>
        /// <param name="commandText">执行的脚本。</param>
        /// <param name="connection">数据库连接。</param>
        /// <param name="parameters">数据库参数的序列。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0表示不指定，此时套用默认的超时设置。</param>
        /// <returns><see cref="DbCommand"/>的实例。</returns>
        protected virtual DbCommand CreateCommand(string commandText,
            DbConnection connection, IEnumerable<DbParameter> parameters,
            CommandType commandType, int timeout)
        {
            var cmd = connection.CreateCommand();

            cmd.CommandType = commandType;
            cmd.CommandText = commandText;

            // timeout 为0时套用默认超时。
            cmd.CommandTimeout = timeout == 0 ? DefaultTimeout : timeout;

            if (parameters != null)
            {
                foreach (var p in parameters)
                    cmd.Parameters.Add(p);
            }

            return cmd;
        }

        /// <summary>
        /// 打开指定的数据库连接。
        /// 此方法在各<see cref="IDbClient"/>方法中的命令执行前被调用，重写此方法以控制其行为。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        protected virtual void OpenConnection(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        /// <summary>
        /// 关闭指定的数据库连接。
        /// 此方法在各<see cref="IDbClient"/>方法中的命令执行后被调用，重写此方法以控制其行为。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        protected virtual void CloseConnection(DbConnection connection)
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }

        private DbConnection CreateAndOpenConnection()
        {
            var connection = CreateConnection();
            OpenConnection(connection);
            return connection;
        }
        private IEnumerable<IDataRecord> YieldRows(DbConnection connection, DbDataReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    yield return reader;
                }
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();

                if (connection != null)
                    CloseConnection(connection);
            }
        }

        // 将查询结果（reader）映射到对象列表，若结果集数据为0行，返回空列表（长度为0）
        private IList<T> MapRowsToList<T>(IDataReader reader, IMapper<T> mapper)
        {
            List<T> results = null;

            // 第一行单独处理，实现 results 的延迟加载，没有读到行就不产生垃圾。
            var rowCount = 1;
            if (reader.Read())
            {
                results = new List<T>();
                var row = mapper.MapRow(reader, rowCount);
                results.Add(row);
            }

            // 读取剩余的行
            while (reader.Read())
            {
                var row = mapper.MapRow(reader, ++rowCount);

                // 这里 results 不会是null了。
                // ReSharper disable once PossibleNullReferenceException
                results.Add(row);
            }

            if (results != null)
                return results;

            return new T[0];
        }

        private DataTable FillDataTable(DbCommand command)
        {
            var dataAdapter = CreateDataAdapter();
            var dataTable = new DataTable();
            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        private DataSet FillDataSet(DbCommand command)
        {
            var dataAdapter = CreateDataAdapter();
            var dataSet = new DataSet();
            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(dataSet);
            return dataSet;
        }

        private DbDataAdapter CreateDataAdapter()
        {
            var dataAdapter = Factory.CreateDataAdapter();

            if (dataAdapter == null)
                throw new NotSupportedException("Cannot create a data-adapter from the underlying DbProviderFactory.");

            return dataAdapter;
        }
    }
}
