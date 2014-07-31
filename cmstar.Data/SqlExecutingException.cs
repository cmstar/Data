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

                    var p = Parameters[i];
                    builder.AppendFormat("PARAM {0} {1}({2}) {3}: {4} ",
                        p.ParameterName, p.DbType, p.Size, p.Direction, p.Value);
                }
            }

            _message = builder.ToString();
            return _message;
        }
    }
}
