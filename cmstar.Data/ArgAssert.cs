using System;
using System.Linq;

namespace cmstar.Data
{
    /// <summary>
    /// ����������֤���йط�����
    /// </summary>
    internal static class ArgAssert
    {
        /// <summary>
        /// ���Բ����ǿ����á�
        /// </summary>
        /// <param name="arg">����ʵ����</param>
        /// <param name="name">�������ơ�</param>
        public static void NotNull(object arg, string name)
        {
            if (arg == null)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        /// �����ַ������͵Ĳ����ǿ����û���ַ�����
        /// </summary>
        /// <param name="value">�ַ���ʵ����</param>
        /// <param name="parameterName">�������ơ�</param>
        public static void NotNullOrEmpty(string value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Length == 0)
                throw new ArgumentException($"The parameter '{parameterName}' should not be an empty string.");
        }

        /// <summary>
        /// �����ַ������͵Ĳ����ǿ����û���ַ�����ֻ�����հ��ַ���
        /// </summary>
        /// <param name="value">�ַ���ʵ����</param>
        /// <param name="parameterName">�������ơ�</param>
        public static void NotNullOrEmptyOrWhitespace(string value, string parameterName)
        {
            NotNullOrEmpty(value, parameterName);

            if (value.All(char.IsWhiteSpace))
                throw new ArgumentException($"The parameter '{parameterName}' must contain non-empty characters.");
        }

        /// <summary>
        /// ������ֵ���͵Ĳ������ڸ�����ֵ��Χ�ı������ڡ�
        /// </summary>
        /// <param name="value">ֵ��</param>
        /// <param name="parameterName">�������ơ�</param>
        /// <param name="min">��ֵ�������Сֵ��</param>
        /// <param name="max">��ֵ��������ֵ��</param>
        public static void Between(int value, string parameterName, int min, int max)
        {
            var msg = $"The parameter '{parameterName}' must be between {min} and {max}, the value is {value}.";
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, msg);
        }
    }
}
