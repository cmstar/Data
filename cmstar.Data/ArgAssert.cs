using System;
using System.Linq;

namespace cmstar.Data
{
    /// <summary>
    /// 包含参数验证的有关方法。
    /// </summary>
    internal static class ArgAssert
    {
        /// <summary>
        /// 断言参数非空引用。
        /// </summary>
        /// <param name="arg">参数实例。</param>
        /// <param name="name">参数名称。</param>
        public static void NotNull(object arg, string name)
        {
            if (arg == null)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        /// 断言字符串类型的参数非空引用或空字符串。
        /// </summary>
        /// <param name="value">字符串实例。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotNullOrEmpty(string value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Length == 0)
                throw new ArgumentException($"The parameter '{parameterName}' should not be an empty string.");
        }

        /// <summary>
        /// 断言字符串类型的参数非空引用或空字符串或只包含空白字符。
        /// </summary>
        /// <param name="value">字符串实例。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotNullOrEmptyOrWhitespace(string value, string parameterName)
        {
            NotNullOrEmpty(value, parameterName);

            if (value.All(char.IsWhiteSpace))
                throw new ArgumentException($"The parameter '{parameterName}' must contain non-empty characters.");
        }

        /// <summary>
        /// 断言数值类型的参数介于给定的值范围的闭区间内。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="min">数值允许的最小值。</param>
        /// <param name="max">数值允许的最大值。</param>
        public static void Between(int value, string parameterName, int min, int max)
        {
            var msg = $"The parameter '{parameterName}' must be between {min} and {max}, the value is {value}.";
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, msg);
        }
    }
}
