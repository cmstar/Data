using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace cmstar.Data
{
    /// <summary>
    /// 表示SQL命令执行过程中的错误。
    /// </summary>
    [Serializable]
    public class SqlExecutingException : Exception
    {
        private string _message;

        /// <summary>
        /// 初始化<see cref="SqlExecutingException"/>的新实例。
        /// </summary>
        /// <param name="commandText">执行的SQL语句。</param>
        /// <param name="commandType">SQL命令类型。</param>
        /// <param name="parameters">参数表。</param>
        /// <param name="innerException">指定引起此错误的异常。</param>
        public SqlExecutingException(
            string commandText,
            CommandType commandType,
            IEnumerable<DbParameter> parameters,
            Exception innerException)
            : this(commandText, commandType, parameters,
                innerException == null ? string.Empty : innerException.Message,
                innerException)
        {
        }

        /// <summary>
        /// 初始化<see cref="SqlExecutingException"/>的新实例。
        /// </summary>
        /// <param name="commandText">执行的SQL语句。</param>
        /// <param name="commandType">SQL命令类型。</param>
        /// <param name="parameters">参数表。</param>
        /// <param name="message">此错误的描述信息。</param>
        /// <param name="innerException">指定引起此错误的异常。</param>
        public SqlExecutingException(
            string commandText,
            CommandType commandType,
            IEnumerable<DbParameter> parameters,
            string message, Exception innerException)
            : base(message, innerException)
        {
            CommandText = commandText;
            CommandType = commandType;
            Parameters = parameters == null ? new DbParameter[0] : parameters.ToArray();
        }

        /// <summary>
        /// 获取出现错误的SQL命令文本。
        /// </summary>
        public string CommandText { get; private set; }

        /// <summary>
        /// 获取SQL命令的执行类型。
        /// </summary>
        public CommandType CommandType { get; private set; }

        /// <summary>
        /// 获取SQL命令的参数集合。
        /// </summary>
        public IList<DbParameter> Parameters { get; private set; }

        public override string Message
        {
            get { return _message ?? (_message = BuildMessage()); }
        }

        private string BuildMessage()
        {
            var builder = new StringBuilder(CommandText.Length * 2);

            if (!string.IsNullOrEmpty(base.Message))
                builder.AppendLine(base.Message);

            builder.Append("The error occured while executing command of type {");
            builder.Append(CommandType).AppendLine("}:");
            builder.Append(CommandText);

            if (Parameters != null && Parameters.Count > 0)
            {
                builder.AppendLine();
                for (int i = 0; i < Parameters.Count; i++)
                {
                    if (i > 0)
                        builder.AppendLine();

                    // PARAM name dbtype(length) direction: value
                    var p = Parameters[i];
                    builder.Append("PARAM");

                    if (p == null)
                    {
                        builder.Append(" Unspecified");
                    }
                    else
                    {
                        builder.Append(" ").Append(p.ParameterName);
                        builder.Append(" ").Append(p.DbType);

                        if (p.Size > 0)
                        {
                            builder.Append("(").Append(p.Size).Append(")");
                        }

                        builder.Append(" ").Append(p.Direction).Append(":");

                        if (p.Value == DBNull.Value)
                        {
                            builder.Append("<NULL>");
                        }
                        else
                        {
                            builder.Append(" ").Append(p.Value);
                        }
                    }

                    builder.Append(" ");
                }
            }

            _message = builder.ToString();
            return _message;
        }
    }
}
