using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using cmstar.Data.Dynamic;

namespace cmstar.Data.Indexing
{
    /// <summary>
    /// 提供<see cref="IDbClient"/>的扩展方法，这些方法允许使用基于索引的SQL参数表以简化编码过程。
    /// </summary>
    /// <remarks>
    /// 这些扩展方法允许SQL中使用类似@n的形式指定参数：
    /// <code>sql = "SELECT id FROM table WHERE name=@0 AND city=@1 AND age>@2";</code>
    /// 其中n对应参数表param中的索引位置为n的值。
    /// <example><code>client.Scalar(sql, "John Doe", "New York", 35);</code></example>
    /// * 参数本身也可以是<see cref="DbParameter"/>的实例，此时参数会被直接使用。
    /// * 这些方法默认<see cref="CommandType"/>为<see cref="CommandType.Text"/>，
    /// 若需要执行存储过程，需要使用类似如下的方式：
    /// <code>sql = "EXEC proc @0, @1, @2";</code>
    /// </remarks>
    public static class IndexingExtension
    {
        /// <summary>
        /// 获取查询的第一行第一列的值。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>查询结果的第一行第一列的值。若查询结果行数为0，返回<c>null</c>。</returns>
        public static object Scalar(this IDbClient client, string sql, params object[] param)
        {
            return client.Scalar(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 执行非查询SQL语句，并返回所影响的行数。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>SQL所影响的行数。</returns>
        public static int Execute(this IDbClient client, string sql, params object[] param)
        {
            return client.Execute(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 执行非查询SQL语句，并断言所影响的行数。若影响的函数不正确，抛出异常。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="expectedSize">被断言的影响行数。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public static void SizedExecute(this IDbClient client, int expectedSize, string sql, params object[] param)
        {
            client.SizedExecute(expectedSize, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataTable"/>。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static DataTable DataTable(this IDbClient client, string sql, params object[] param)
        {
            return client.DataTable(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataSet"/>。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static DataSet DataSet(this IDbClient client, string sql, params object[] param)
        {
            return client.DataSet(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 判断给定的查询的结果是否至少包含1行。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>若查询结果至少包含1行，返回<c>true</c>；否则返回<c>false</c>。</returns>
        public static bool Exists(this IDbClient client, string sql, params object[] param)
        {
            return client.Exists(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 获取查询结果的第一行记录。
        /// 若查询命中的行数为0，返回null。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns><see cref="IDataRecord"/>的实现，包含查询的第一行记录。</returns>
        public static IDataRecord GetRow(this IDbClient client, string sql, params object[] param)
        {
            return client.GetRow(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 获取查询结果的第一行记录，以数组形式返回记录内各列的值。
        /// 数组元素顺序与列顺序一致。若查询命中的行数为0，返回null。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <returns>包含了各列的值的数组。</returns>
        public static object[] ItemArray(this IDbClient client, string sql, params object[] param)
        {
            return client.ItemArray(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <returns>目标类型的实例。</returns>
        public static T Get<T>(this IDbClient client, IMapper<T> mapper, string sql, params object[] param)
        {
            return client.Get(mapper, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// SQL命中的记录必须为1行，否则抛出异常。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static T ForceGet<T>(this IDbClient client, IMapper<T> mapper, string sql, params object[] param)
        {
            return client.ForceGet(mapper, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象的集合。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例的集合。若查询命中的行数为0，返回空集合。</returns>
        public static IList<T> List<T>(this IDbClient client, IMapper<T> mapper, string sql, params object[] param)
        {
            return client.List(mapper, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 获取查询结果得行序列。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>查询结果得行序列。</returns>
        public static IEnumerable<IDataRecord> Rows(this IDbClient client, string sql, params object[] param)
        {
            return client.Rows(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 查询并根据结果创建目标类型的实例。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例。</returns>
        public static T Get<T>(this IDbClient client, string sql, params object[] param)
        {
            var identity = new CommandIdentity(client.ConnectionString, string.Empty, null, null, typeof(T));
            var cache = CommandCache.Get(identity);
            if (cache != null)
                return Get(client, (IMapper<T>)cache.Mapper, sql, param);

            var rows = Rows(client, sql, param);
            foreach (var row in rows)
            {
                var mapper = MapperParser.Parse<T>(row);
                CommandCache.Set(identity, new CommandCacheItem { Mapper = mapper });
                return mapper.MapRow(row, 1);
            }

            return default(T);
        }

        /// <summary>
        /// 查询并根据结果创建目标类型的实例。
        /// SQL命中的记录必须为1行，否则抛出异常。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static T ForceGet<T>(this IDbClient client, string sql, params object[] param)
        {
            var identity = new CommandIdentity(client.ConnectionString, string.Empty, null, null, typeof(T));
            var cache = CommandCache.Get(identity);
            if (cache != null)
                return ForceGet(client, (IMapper<T>)cache.Mapper, sql, param);

            var dbParam = GenerateParameters(client, param).ToList();
            var rows = client.Rows(sql, dbParam);
            var rowCount = 0;
            var res = default(T);

            foreach (var row in rows)
            {
                if (rowCount == 0)
                {
                    var mapper = MapperParser.Parse<T>(row);
                    CommandCache.Set(identity, new CommandCacheItem { Mapper = mapper });
                    res = mapper.MapRow(row, 1);
                }

                rowCount++;
            }

            if (rowCount == 1)
                return res;

            throw new IncorrectResultSizeException(sql, CommandType.Text, dbParam, 1, rowCount);
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例集合，由一个模板对象指定目标类型。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="template">用于指定目标类型的模板对象。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static IList<T> TemplateList<T>(this IDbClient client, T template, string sql, params object[] param)
        {
            return Query<T>(client, sql, param).ToList();
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例集合。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static IList<T> List<T>(this IDbClient client, string sql, params object[] param)
        {
            return Query<T>(client, sql, param).ToList();
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例序列，由一个模板对象指定目标类型。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="template">用于指定目标类型的模板对象。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static IEnumerable<T> TemplateQuery<T>(this IDbClient client, T template, string sql, params object[] param)
        {
            return Query<T>(client, sql, param);
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例序列。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例序列。</returns>
        public static IEnumerable<T> Query<T>(this IDbClient client, string sql, params object[] param)
        {
            var identity = new CommandIdentity(client.ConnectionString, string.Empty, null, null, typeof(T));
            var cache = CommandCache.Get(identity);

            var mapper = cache == null ? null : (IMapper<T>)cache.Mapper;
            var rowNum = 0;

            // 展开foreach循环，使mapper == null的仅判断一次
            using (var iter = Rows(client, sql, param).GetEnumerator())
            {
                if (!iter.MoveNext())
                    yield break;

                if (mapper == null)
                {
                    mapper = MapperParser.Parse<T>(iter.Current);
                    CommandCache.Set(identity, new CommandCacheItem { Mapper = mapper });
                }

                do
                {
                    yield return mapper.MapRow(iter.Current, ++rowNum);

                } while (iter.MoveNext());
            }
        }

        private static IEnumerable<DbParameter> GenerateParameters(IDbClient client, object[] param)
        {
            for (int i = 0; i < param.Length; i++)
            {
                var value = param[i];
                var p = value as DbParameter;

                if (p != null)
                    yield return p;

                p = client.CreateParameter();
                p.ParameterName = i.ToString(CultureInfo.InvariantCulture);

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

                    p.Value = value;
                    p.DbType = dbType;
                    if (value is string)
                    {
                        p.Size = 4000;
                    }
                }

                yield return p;
            }
        }
    }
}
