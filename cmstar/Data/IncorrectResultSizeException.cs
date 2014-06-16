using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// 表示数据库操作所影响的行数不正确的异常。
    /// </summary>
    public class IncorrectResultSizeException : SqlExecutingException
    {
        /// <summary>
        /// 数据库操作应该影响的行数。
        /// </summary>
        public virtual int ExpectedSize { get; private set; }

        /// <summary>
        /// 数据库操作实际影响的行数。
        /// </summary>
        public virtual int ActualSize { get; private set; }

        /// <summary>
        /// 初始化<see cref="IncorrectResultSizeException"/>的新实例。
        /// </summary>
        /// <param name="commandText">引起错误的SQL命令文本。</param>
        /// <param name="commandType">SQL命令的执行类型。</param>
        /// <param name="parameters">SQL命令的参数序列。</param>
        /// <param name="expectedSize">数据库操作应该影响的行数。</param>
        /// <param name="actualSize">数据库操作实际影响的行数。</param>
        public IncorrectResultSizeException(string commandText, CommandType commandType, IEnumerable<DbParameter> parameters,
            int expectedSize, int actualSize)
            : base(commandText, commandType, parameters,
                string.Format("Affected rows expected: {0}, acttually: {1}.", expectedSize, actualSize),
                null)
        {
            ExpectedSize = expectedSize;
            ActualSize = actualSize;
        }
    }
}
