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
        /// 参考类型，用于判断<see cref="DbParameter.DbType"/>的值。
        /// 若为 null，且<paramref name="value"/>不为 null，则使用 value.GetType() 作为参考类型；
        /// 若<paramref name="value"/>也为 null，则使用<see cref="UnknownTypeDbNull"/>方法的返回值。
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
            // 若值为 null，又没有给定参考类型，就不能知道是何类型的，只能固定给一个值。
            if (value == null && typeRef == null)
                return UnknownTypeDbNull(client);

            // 字符串需要单独对待。
            var dbString = value as DbString;
            if (dbString != null)
                return CreateParameterForDbString(client, dbString);

            // 如果值本身就是 DbParameter，直接采用之，不用创建新的了。
            var param = value as DbParameter;
            if (param != null)
                return param;

            return CreateParameterForMultipleTypes(client, value, typeRef);
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

        private static DbParameter CreateParameterForMultipleTypes(IDbClient client, object value, Type typeRef)
        {
            var type = typeRef ?? value.GetType();
            var dbType = DbTypeConvert.LookupDbType(type);

            if (dbType == DbTypeConvert.NotSupporteDbType)
                throw new NotSupportedException($"The type {type} can not be converted to DbType.");

            var p = client.CreateParameter();

            if (value == null)
            {
                // 小心 null，需要转为 DBNull，否则值为 CLR null 的参数会被忽略。
                p.Value = DBNull.Value;

                // 对于 null 且类型是字符串的情况，总是给定 AnsiString。
                // 因为 String 可以兼容 AnsiString，反向却不一定。
                p.DbType = dbType == DbType.String ? DbType.AnsiString : dbType;
            }
            else
            {
                p.DbType = dbType;
                p.Value = value;

                var stringValue = value as string;
                if (stringValue != null && stringValue.Length <= DbString.DefaultLength)
                {
                    p.Size = DbString.DefaultLength;
                }
            }

            return p;
        }
    }
}
