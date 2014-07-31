using System;
using System.Collections.Generic;
using System.Linq;

namespace cmstar.Data
{
    /// <summary>
    /// 包含参数验证的有关方法。
    /// </summary>
    public static class ArgAssert
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
                throw new ArgumentException("参数\"{0}\"的值不能为空。".FormatWith(parameterName));
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
                throw new ArgumentException("参数\"{0}\"的值不能仅包含空白字符。".FormatWith(parameterName));
        }

        /// <summary>
        /// 断言集合类型的参数非空切至少包含一个元素。
        /// </summary>
        /// <typeparam name="T">集合内元素的类型。</typeparam>
        /// <param name="collection">集合参数的实例。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotNullOrEmpty<T>(ICollection<T> collection, string parameterName)
        {
            if (collection == null)
                throw new ArgumentNullException(parameterName);

            if (collection.Count == 0)
                throw new ArgumentException("集合\"{0}\"必须包含至少一个元素。".FormatWith(parameterName));
        }

        /// <summary>
        /// 断言数值类型的参数不小于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotNegative(double value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能小于0。当前值为 {1}。".FormatWith(parameterName, value);

            NotNegative(value, parameterName, msg);
        }


        /// <summary>
        /// 断言数值类型的参数不小于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void NotNegative(double value, string parameterName, string errorMsg)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数不小于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotNegative(int value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能小于0。当前值为 {1}。".FormatWith(parameterName, value);

            NotNegative(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数不小于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void NotNegative(int value, string parameterName, string errorMsg)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数不小于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotNegative(decimal value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能小于0。当前值为 {1}。".FormatWith(parameterName, value);

            NotNegative(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数不小于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void NotNegative(decimal value, string parameterName, string errorMsg)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数不等于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotZero(double value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能等于0。当前值为 {1}。".FormatWith(parameterName, value);

            NotZero(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数不等于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void NotZero(double value, string parameterName, string errorMsg)
        {
            if (0.00D.Equals(value))
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数不等于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotZero(int value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能等于0。当前值为 {1}。".FormatWith(parameterName, value);

            NotZero(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数不等于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void NotZero(int value, string parameterName, string errorMsg)
        {
            if (value == 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数不等于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void NotZero(decimal value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能等于0。当前值为 {1}。".FormatWith(parameterName, value);

            NotZero(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数不等于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void NotZero(decimal value, string parameterName, string errorMsg)
        {
            if (value == 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数大于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void IsPositive(double value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能小于或等于0。当前值为 {1}。".FormatWith(parameterName, value);

            IsPositive(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数大于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void IsPositive(double value, string parameterName, string errorMsg)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数大于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void IsPositive(int value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能小于或等于0。当前值为 {1}。".FormatWith(parameterName, value);

            IsPositive(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数大于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void IsPositive(int value, string parameterName, string errorMsg)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数大于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        public static void IsPositive(decimal value, string parameterName)
        {
            var msg = "参数\"{0}\"的值不能小于或等于0。当前值为 {1}。".FormatWith(parameterName, value);

            IsPositive(value, parameterName, msg);
        }

        /// <summary>
        /// 断言数值类型的参数大于0。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void IsPositive(decimal value, string parameterName, string errorMsg)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数介于给定的值范围的闭区间内。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="min">数值允许的最小值。</param>
        /// <param name="max">数值允许的最大值。</param>
        public static void Between(double value, string parameterName, double min, double max)
        {
            var msg = "参数\"{0}\"的值必须介于{1}-{2}间。当前值为 {3}。".FormatWith(parameterName, min, max, value);

            Between(value, parameterName, min, max, msg);
        }

        /// <summary>
        /// 断言数值类型的参数介于给定的值范围的闭区间内。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="min">数值允许的最小值。</param>
        /// <param name="max">数值允许的最大值。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void Between(double value, string parameterName, double min, double max, string errorMsg)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
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
            var msg = "参数\"{0}\"的值必须介于{1}-{2}间。当前值为 {3}。".FormatWith(parameterName, min, max, value);

            Between(value, parameterName, min, max, msg);
        }

        /// <summary>
        /// 断言数值类型的参数介于给定的值范围的闭区间内。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="min">数值允许的最小值。</param>
        /// <param name="max">数值允许的最大值。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void Between(int value, string parameterName, int min, int max, string errorMsg)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言数值类型的参数介于给定的值范围的闭区间内。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="min">数值允许的最小值。</param>
        /// <param name="max">数值允许的最大值。</param>
        public static void Between(decimal value, string parameterName, int min, int max)
        {
            var msg = "参数\"{0}\"的值必须介于{1}-{2}间。当前值为 {3}。".FormatWith(parameterName, min, max, value);

            Between(value, parameterName, min, max, msg);
        }

        /// <summary>
        /// 断言数值类型的参数介于给定的值范围的闭区间内。
        /// </summary>
        /// <param name="value">值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="min">数值允许的最小值。</param>
        /// <param name="max">数值允许的最大值。</param>
        /// <param name="errorMsg">当断言不成立时的提示消息。</param>
        public static void Between(decimal value, string parameterName, int min, int max, string errorMsg)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, errorMsg);
        }

        /// <summary>
        /// 断言参数类型匹配给定的类型。
        /// 注意<c>null</c>可以匹配任意类新，若<paramref name="arg"/>为<c>null</c>，则此方法不会抛出任何异常。
        /// </summary>
        /// <param name="arg">参数值。</param>
        /// <param name="type">参数需匹配的类型。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="canBeInheritted">
        /// 指定是否可使用参数类型的基类匹配给定类型。若设置为<c>false</c>，则参数类型与给定类型必须严格相等。
        /// 默认值为<c>true</c>。
        /// </param>
        public static void IsType(object arg, Type type, string parameterName, bool canBeInheritted = true)
        {
            if (arg == null)
                return;

            var t = arg.GetType();

            if (t == type || (canBeInheritted && t.IsSubclassOf(type)))
                return;

            var msg = "参数\"{0}\"必须是{0}或其子类。".FormatWith(parameterName, type.FullName);
            throw new ArgumentException(msg, parameterName);
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
