using System;

namespace cmstar.Data
{
    /// <summary>
    /// 表示数据过期的错误。
    /// </summary>
    public class DataExpiredException : Exception
    {
        private static readonly string DefaultMessage
            = "当前的数据可能已经过期，请刷新或重试。" + Environment.NewLine
            + "若问题一直存在，请联系技术支持人员。";

        /// <summary>
        /// 初始化类型的新实例。使用默认的异常提示信息。
        /// </summary>
        public DataExpiredException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// 使用指定的内部异常初始化类新的哦新实例。使用默认的异常提示信息。
        /// </summary>
        /// <param name="innerException">内部异常的实例。</param>
        public DataExpiredException(Exception innerException)
            : base(DefaultMessage, innerException)
        {
        }
    }
}
