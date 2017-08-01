using System.Data;

namespace cmstar.Data
{
    /// <summary>
    /// 用于记录传递给数据库字符串的值。
    /// 包含<see cref="IsAnsi"/>属性用于区分字符串是否是 Unicode 字符。
    /// </summary>
    /// <remarks>
    /// <see cref="DbType"/>包含<see cref="DbType.String"/>和<see cref="DbType.AnsiString"/>
    /// 分别表示 Unicode 字符与非 Unicode 字符，但 .net 中的字符串只有一种，为了区分它们，只能
    /// 定义一个独立的类型表示。
    /// </remarks>
    public class DbString
    {
        /// <summary>
        /// 默认的长度，值为 4000。
        /// </summary>
        public const int DefaultLength = 4000;

        /// <summary>
        /// 是否是定长的字符串。
        /// </summary>
        public bool IsFixedLength { get; set; }

        /// <summary>
        /// 字符串的长度。默认为 -1，表示使用默认长度：
        /// 字符串长度不多于<see cref="DefaultLength"/>，则为<see cref="DefaultLength"/>；
        /// 否则，使用最大长度。
        /// </summary>
        public int Length { get; set; } = -1;

        /// <summary>
        /// 是否仅包含非 Unicode 字符。默认为 false。
        /// </summary>
        public bool IsAnsi { get; set; }

        /// <summary>
        /// 字符串的值。
        /// </summary>
        public string Value { get; set; }
    }
}
