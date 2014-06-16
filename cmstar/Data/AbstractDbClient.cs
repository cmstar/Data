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
    public abstract class AbstractDbClient : IDbClient
    {
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
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>查询结果的第一行第一列的值。若查询结果行数为0，返回<c>null</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public object Scalar(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, "sql");

            DbConnection connection = null;
            try
            {
                connection = CreateAndOpenConnection();
                var cmd = CreateCommand(connection, parameters, commandType, timeOut);
                cmd.CommandText = sql;
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
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
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public int Execute(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, "sql");

            DbConnection connection = null;
            try
            {
                connection = CreateAndOpenConnection();
                var cmd = CreateCommand(connection, parameters, commandType, timeOut);
                cmd.CommandText = sql;
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
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
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public void SizedExecute(int expectedSize,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var actualSize = Execute(sql, null, commandType, timeOut);
            if (actualSize != expectedSize)
                throw new IncorrectResultSizeException(sql, commandType, parameters, expectedSize, actualSize);
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataTable"/>。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public DataTable DataTable(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return DataSet(sql, null, commandType, timeOut).Tables[0];
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataSet"/>。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataSet"/>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public DataSet DataSet(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, "sql");

            var ds = new DataSet();
            FillDataSet(ds, sql, parameters, commandType, timeOut);
            return ds;
        }

        /// <summary>
        /// 判断给定的查询的结果是否至少包含1行。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>若查询结果至少包含1行，返回<c>true</c>；否则返回<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        public bool Exists(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return Scalar(sql, parameters, commandType, timeOut) != null;
        }

        /// <summary>
        /// 获取查询结果的第一行记录。
        /// 若查询命中的行数为0，返回null。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns><see cref="IDataRecord"/>的实现，包含查询的第一行记录。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        /// <remarks>
        /// 区别于<see cref="DbCommand.ExecuteReader()"/>的用法，此方法执行完毕后将并不保持数据库连接，
        /// 也不需要调用<see cref="IDisposable.Dispose"/>。
        /// </remarks>
        public IDataRecord GetRow(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return Get(new SingleRowKeeperMapper(), sql, parameters, commandType, timeOut);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <returns>目标类型的实例。</returns>
        public T Get<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, "sql");
            ArgAssert.NotNull(mapper, "mapper");

            var rows = Rows(sql, parameters, commandType, timeOut);
            foreach (var row in rows)
            {
                return mapper.MapRow(row, 1);
            }

            return default(T);
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
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public T ForceGet<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var res = List(mapper, sql, parameters, commandType, timeOut);

            if (res.Count != 1)
                throw new IncorrectResultSizeException(sql, commandType, parameters, 1, res.Count);

            return res[0];
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象的集合。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例的集合。若查询命中的行数为0，返回空集合。</returns>
        public IList<T> List<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, "sql");
            ArgAssert.NotNull(mapper, "mapper");

            var results = new List<T>();
            DbConnection connection = null;
            IDataReader reader = null;
            try
            {
                connection = CreateAndOpenConnection();

                var cmd = CreateCommand(connection, parameters, commandType, timeOut);
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();
                var rowCount = 0;
                while (reader.Read())
                {
                    var row = mapper.MapRow(reader, ++rowCount);
                    results.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null)
                    CloseConnection(connection);
            }

            return results;
        }

        /// <summary>
        /// 获取查询结果得行序列。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>查询结果得行序列。</returns>
        public IEnumerable<IDataRecord> Rows(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            ArgAssert.NotNullOrEmpty(sql, "sql");

            DbConnection connection = null;
            DbDataReader reader = null;

            try
            {
                connection = CreateAndOpenConnection();
                var cmd = CreateCommand(connection, parameters, commandType, timeOut);
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                //获取reader失败时，关闭连接
                if (reader == null && connection != null)
                    CloseConnection(connection);
            }

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

        /// <summary>
        /// 创建事务容器。
        /// </summary>
        /// <returns><see cref="ITransactionKeeper"/>。</returns>
        public virtual ITransactionKeeper CreateTransaction()
        {
            return ThreadLocalTransactionKeeper.OpenTransaction(Factory, ConnectionString);
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
        /// <param name="connection">数据库连接。</param>
        /// <param name="parameters">数据库参数的序列。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns><see cref="DbCommand"/>的实例。</returns>
        protected virtual DbCommand CreateCommand(
            DbConnection connection, IEnumerable<DbParameter> parameters,
            CommandType commandType, int timeOut)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandTimeout = timeOut;

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

        private void FillDataSet(DataSet dataSet,
            string sql, IEnumerable<DbParameter> parameters,
            CommandType commandType, int timeOut)
        {
            DbConnection connection = null;
            try
            {
                connection = CreateAndOpenConnection();
                var cmd = CreateCommand(connection, parameters, commandType, timeOut);
                cmd.CommandText = sql;

                var dataAdapter = Factory.CreateDataAdapter();
                if (dataAdapter == null)
                    throw new NotSupportedException("获取数据适配器失败。");

                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                throw new SqlExecutingException(sql, commandType, parameters, ex);
            }
            finally
            {
                if (connection != null)
                    CloseConnection(connection);
            }
        }

        private DbConnection CreateAndOpenConnection()
        {
            var connection = CreateConnection();
            OpenConnection(connection);
            return connection;
        }
    }
}
