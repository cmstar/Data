using System;
using System.Data;
using System.Data.Common;
using cmstar.Data.Dynamic;

namespace cmstar.Data
{
    /// <summary>
    /// 包用于创建<see cref="DbParameter"/>的有关扩展方法。
    /// </summary>
    public static class DbClientParamEx
    {
        /// <summary>
        /// 返回一个表示非 Unicode 字符串的<see cref="DbString"/>。
        /// </summary>
        /// <param name="value">字符串的值。</param>
        /// <param name="length">
        /// 指定字符串的长度。<c>-1</c>表示使用<see cref="DbString"/>的默认长度。
        /// 若<paramref name="isFixedLength"/>为<c>true</c>，则必须指定长度。
        /// </param>
        /// <param name="isFixedLength">是否是定长字符串。</param>
        /// <returns><see cref="DbString"/>的实例，其<see cref="DbString.IsAnsi"/>为<c>false</c>。</returns>
        public static DbString AnsiString(this string value, int length = -1, bool isFixedLength = false)
        {
            if (isFixedLength && length <= 0)
                throw new ArgumentException("The length must be specified if the string is fixed-length.");

            return new DbString
            {
                Value = value,
                IsAnsi = true,
                IsFixedLength = isFixedLength,
                Length = length
            };
        }

        /// <summary>
        /// 返回一个值为<see cref="DBNull.Value"/>的<see cref="DbParameter"/>的新实例。
        /// 给定默认的<see cref="DbParameter.DbType"/>值。
        /// 仅在不能判定是使用何种<see cref="DbType"/>时使用此方法。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter UnknownTypeDbNull(this IDbClient client)
        {
            var p = client.CreateParameter();
            p.Value = DBNull.Value;
            p.DbType = DbType.AnsiString;
            p.Size = 1;
            return p;
        }

        /// <summary>
        /// 创建一个SQL参数实例，并指定参数的有关信息。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="dbType">参数的数据类型。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="size">参数的大小。</param>
        /// <param name="direction">参数的输入、输出类型。</param>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter CreateParameter(this IDbClient client,
            string name, DbType dbType, object value = null, int size = 0,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var p = client.CreateParameter();

            p.ParameterName = name;
            p.DbType = dbType;
            p.Size = size;
            p.Value = value ?? DBNull.Value;

            return p;
        }

        /// <summary>
        /// 创建一个SQL参数实例。SQL参数的类型由参数的值决定。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="typeRef">
        /// 当<paramref name="value"/>不为 null 时，使用其类型确定<see cref="DbParameter.DbType"/>的值；
        /// 当<paramref name="value"/>为 null 时，使用<paramref name="typeRef"/>确定<see cref="DbParameter.DbType"/>的值；
        /// 若<paramref name="value"/>和<paramref name="typeRef"/>都是 null 时，使用<see cref="UnknownTypeDbNull"/>方法的返回值。
        /// </param>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter CreateParameter(this IDbClient client, string name, object value, Type typeRef = null)
        {
            var p = CreateParameterInternal(client, value, typeRef);
            p.ParameterName = name;
            return p;
        }

        private static DbParameter CreateParameterInternal(IDbClient client, object value, Type typeRef)
        {
            // DbString 作为一个内置的特殊类型，单独处理。
            if (value is DbString dbString)
                return CreateParameterForDbString(client, dbString);

            // 如果值本身就是 DbParameter，直接采用之，不用创建新的了。
            if (value is DbParameter dbParameter)
                return dbParameter;

            DbType dbType;
            DbParameter p;

            if (value == null)
            {
                // 若值为 null，又没有给定参考类型，就不能知道是何类型的，只能固定给一个值。
                if (typeRef == null)
                    return UnknownTypeDbNull(client);

                dbType = LookupDbType(typeRef);
                p = client.CreateParameter();

                // 小心 null，需要转为 DBNull，否则值为 CLR null 的参数会被忽略。
                p.Value = DBNull.Value;

                // 对于 null 且类型是字符串的情况，总是给定 AnsiString。
                // 因为 String 可以兼容 AnsiString，反向却不一定。
                p.DbType = dbType == DbType.String ? DbType.AnsiString : dbType;

                return p;
            }

            dbType = LookupDbType(value.GetType());
            p = client.CreateParameter();

            p.DbType = dbType;
            p.Value = value;

            // 若不指定字符串长度，参数带有的默认长度可能导致两种情况：
            // 1. 参数内指定的长度（Size）等于参数值的长度（Length）；
            // 2. 参数内指定的长度等价于 MAX 。
            // 由于每次给定的值可能不一样，不定长或 MAX 长度会导致如 SQLServer 数据库执行计划不能复用，进而影响性能。
            // 因为这里不能知道数据库的字段定义是多长，只能给出一个较为稳定的长度：
            // 1. 它足够长，多数的值长度都能涵盖在内；
            // 2. 它不会太长，例如长于 SQLServer 所允许的 NVARCHAR 列的最大可索引长度 4000 。
            if (value is string stringValue && stringValue.Length <= DbString.DefaultLength)
            {
                p.Size = DbString.DefaultLength;
            }

            return p;
        }

        private static DbType LookupDbType(Type type)
        {
            var dbType = DbTypeConvert.LookupDbType(type);
            if (dbType == DbTypeConvert.NotSupporteDbType)
                throw new NotSupportedException($"The type {type} can not be converted to DbType.");

            return dbType;
        }

        private static DbParameter CreateParameterForDbString(IDbClient client, DbString dbString)
        {
            if (dbString.IsFixedLength && dbString.Length <= 0)
                throw new InvalidOperationException("The length must be specified if the string is fixed-length.");

            var param = client.CreateParameter();

            // 小心 null，需要转为 DBNull，否则值为 CLR null 的参数会被忽略。
            param.Value = (object)dbString.Value ?? DBNull.Value;

            // AnsiString / IsFixedLength 总共4种情况。
            param.DbType = dbString.IsFixedLength
                ? (dbString.IsAnsi ? DbType.AnsiStringFixedLength : DbType.StringFixedLength)
                : (dbString.IsAnsi ? DbType.AnsiString : DbType.String);

            if (dbString.Length == -1
                && dbString.Value != null
                && dbString.Value.Length <= DbString.DefaultLength)
            {
                param.Size = DbString.DefaultLength;
            }
            else
            {
                param.Size = dbString.Length;
            }

            return param;
        }
    }
}
