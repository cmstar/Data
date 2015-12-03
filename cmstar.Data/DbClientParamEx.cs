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
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter CreateParameter(this IDbClient client, string name, object value)
        {
            var p = client.CreateParameter();

            p.ParameterName = name;

            if (value == null)
            {
                p.Value = DBNull.Value;
                p.DbType = DbType.AnsiString;
                p.Size = 1;
            }
            else
            {
                var dbType = DbTypeConvert.LookupDbType(value.GetType());

                if (dbType == DbTypeConvert.NotSupporteDbType)
                {
                    throw new NotSupportedException(string.Format(
                        "The type {0} can not be converted to a DbType.", value.GetType()));
                }

                p.DbType = dbType;
                p.Value = value;

                var stringValue = value as string;
                if (stringValue != null && stringValue.Length <= DbTypeConvert.DefaultStringSizeForDbParameter)
                {
                    p.Size = DbTypeConvert.DefaultStringSizeForDbParameter;
                }
            }

            return p;
        }
    }
}
