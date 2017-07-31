using System;
using System.Data;
using System.Data.Common;
using cmstar.Data.Dynamic;

namespace cmstar.Data
{
    /// <summary>
    /// 包含从<see cref="IDbClient"/>创建<see cref="DbParameter"/>的有关扩展方法。
    /// </summary>
    public static class DbClientParamEx
    {
        /// <summary>
        /// 返回一个值为<see cref="DBNull.Value"/>的<see cref="DbParameter"/>。
        /// 给定默认的<see cref="DbParameter.DbType"/>值。
        /// 仅在不能判定是使用何种<see cref="DbType"/>时使用此方法。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter UntypedDbNull(this IDbClient client)
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
        /// 若<paramref name="value"/>也为 null，则使用<see cref="UntypedDbNull"/>方法的返回值。
        /// </param>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter CreateParameter(this IDbClient client, string name, object value, Type typeRef = null)
        {
            DbParameter p;

            // 若值为 null，又没有给定参考类型，就不能知道是何类型的，只能固定给一个值。
            if (value == null && typeRef == null)
            {
                p = UntypedDbNull(client);
            }
            else
            {
                // 如果值本身就是 DbParameter，直接采用之，不用创建新的了。
                p = value as DbParameter ?? PrepareDbParameter(client, value, typeRef);
            }

            p.ParameterName = name;
            return p;
        }

        private static DbParameter PrepareDbParameter(IDbClient client, object value, Type typeRef)
        {
            var type = typeRef ?? value.GetType();
            var dbType = DbTypeConvert.LookupDbType(type);

            if (dbType == DbTypeConvert.NotSupporteDbType)
                throw new NotSupportedException($"The type {type} can not be converted to a DbType.");

            var p = client.CreateParameter();
            p.DbType = dbType;
            p.Value = value;

            var stringValue = value as string;
            if (stringValue != null && stringValue.Length <= DbTypeConvert.DefaultStringSizeForDbParameter)
            {
                p.Size = DbTypeConvert.DefaultStringSizeForDbParameter;
            }

            return p;
        }
    }
}
