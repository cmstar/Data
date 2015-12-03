using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// 定义数据库访问客户端。
    /// </summary>
    public interface IDbClient
    {
        /// <summary>
        /// 获取当前实例所使用的数据库连接字符串。
        /// </summary>
        string ConnectionString { get; }

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
        object Scalar(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// 执行非查询SQL语句，并返回所影响的行数。
        /// </summary>
        /// <param name="sql">非查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>SQL所影响的行数。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        int Execute(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// 执行非查询SQL语句，并断言所影响的行数。若影响的函数不正确，抛出异常。
        /// </summary>
        /// <param name="expectedSize">被断言的影响行数。</param>
        /// <param name="sql">非查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        void SizedExecute(int expectedSize,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

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
        DataTable DataTable(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

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
        DataSet DataSet(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

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
        bool Exists(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

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
        IDataRecord GetRow(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// 获取查询结果的第一行记录，以数组形式返回记录内各列的值。
        /// 数组元素顺序与列顺序一致。若查询命中的行数为0，返回null。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>包含了各列的值的数组。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="sql"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException">当<paramref name="sql"/>长度为0。</exception>
        object[] ItemArray(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

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
        T Get<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

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
        T ForceGet<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

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
        IList<T> List<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// 获取查询结果得行序列。
        /// </summary>
        /// <param name="sql">查询SQL。</param>
        /// <param name="parameters">参数序列。空序列或null表示没有参数。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>查询结果得行序列。</returns>
        IEnumerable<IDataRecord> Rows(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// 创建事务容器。
        /// </summary>
        /// <returns><see cref="ITransactionKeeper"/>。</returns>
        ITransactionKeeper CreateTransaction();

        /// <summary>
        /// 创建一个新的SQL参数实例。
        /// </summary>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        DbParameter CreateParameter();
    }
}