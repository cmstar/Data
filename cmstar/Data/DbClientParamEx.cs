using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// 包含从<see cref="IDbClient"/>创建<see cref="DbParameter"/>的有关扩展方法。
    /// </summary>
    public static class DbClientParamEx
    {
        /// <summary>
        /// 创建一直SQL参数实例，并指定参数的有关信息。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="dbType">参数的数据类型。</param>
        /// <param name="value">参数的值。</param>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter CreateParameter(this IDbClient client,
            string name, DbType dbType, object value = null)
        {
            var p = client.CreateParameter();

            p.ParameterName = name;
            p.DbType = dbType;
            p.Value = value;

            return p;
        }

        /// <summary>
        /// 创建一直SQL参数实例，并指定参数的有关信息。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="dbType">参数的数据类型。</param>
        /// <param name="size">参数的大小。</param>
        /// <param name="value">参数的值。</param>
        /// <returns><see cref="DbParameter"/>的实例。</returns>
        public static DbParameter CreateParameter(this IDbClient client,
            string name, DbType dbType, int size, object value = null)
        {
            var p = client.CreateParameter();

            p.ParameterName = name;
            p.DbType = dbType;
            p.Size = size;
            p.Value = value;

            return p;
        }
    }
}
