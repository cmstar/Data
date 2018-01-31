using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using cmstar.Data.Dynamic;

namespace cmstar.Data
{
    // 实现 IndexingExtension 扩展方法的异步版本
    public static partial class IndexingExtension
    {
        /// <summary>
        /// 获取查询的第一行第一列的值。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>查询结果的第一行第一列的值。若查询结果行数为0，返回<c>null</c>。</returns>
        public static Task<object> IxScalarAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.ScalarAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 执行非查询SQL语句，并返回所影响的行数。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>SQL所影响的行数。</returns>
        public static Task<int> IxExecuteAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.ExecuteAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 执行非查询SQL语句，并断言所影响的行数。若影响的函数不正确，抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="expectedSize">被断言的影响行数。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public static Task IxSizedExecuteAsync(this IDbClient client, int expectedSize, string sql, params object[] param)
        {
            return client.SizedExecuteAsync(expectedSize, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataTable"/>。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static Task<DataTable> IxDataTableAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.DataTableAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataSet"/>。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static Task<DataSet> IxDataSetAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.DataSetAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 判断给定的查询的结果是否至少包含1行。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>若查询结果至少包含1行，返回<c>true</c>；否则返回<c>false</c>。</returns>
        public static Task<bool> IxExistsAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.ExistsAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 获取查询结果的第一行记录。
        /// 若查询命中的行数为0，返回null。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns><see cref="IDataRecord"/>的实现，包含查询的第一行记录。</returns>
        public static Task<IDataRecord> IxGetRowAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.GetRowAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 获取查询结果的第一行记录，以数组形式返回记录内各列的值。
        /// 数组元素顺序与列顺序一致。若查询命中的行数为0，返回null。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <returns>包含了各列的值的数组。</returns>
        public static Task<object[]> IxItemArrayAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.ItemArrayAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <returns>目标类型的实例。</returns>
        public static Task<T> IxGetAsync<T>(this IDbClient client, IMapper<T> mapper, string sql, params object[] param)
        {
            return client.GetAsync(mapper, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// SQL命中的记录必须为1行，否则抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static Task<T> IxForceGetAsync<T>(this IDbClient client, IMapper<T> mapper, string sql, params object[] param)
        {
            return client.ForceGetAsync(mapper, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象的集合。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例的集合。若查询命中的行数为0，返回空集合。</returns>
        public static Task<IList<T>> IxListAsync<T>(this IDbClient client, IMapper<T> mapper, string sql, params object[] param)
        {
            return client.ListAsync(mapper, sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 获取查询结果得行序列。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>查询结果得行序列。</returns>
        public static Task<IEnumerable<IDataRecord>> IxRowsAsync(this IDbClient client, string sql, params object[] param)
        {
            return client.RowsAsync(sql, GenerateParameters(client, param));
        }

        /// <summary>
        /// 查询并根据结果创建目标类型的实例。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例。</returns>
        public static async Task<T> IxGetAsync<T>(this IDbClient client, string sql, params object[] param)
        {
            var identity = new CommandIdentity(client.ConnectionString, sql, null, null, typeof(T));
            var cache = CommandCache.Get(identity);
            if (cache != null)
                return await IxGetAsync(client, (IMapper<T>)cache.Mapper, sql, param);

            var row = await IxGetRowAsync(client, sql, param);
            if (row == null)
                return default(T);

            var mapper = MapperParser.Parse<T>(row);
            CommandCache.Set(identity, new CommandCacheItem { Mapper = mapper });
            return mapper.MapRow(row, 1);
        }

        /// <summary>
        /// 查询并根据结果创建目标类型的实例。
        /// SQL命中的记录必须为1行，否则抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static async Task<T> IxForceGetAsync<T>(this IDbClient client, string sql, params object[] param)
        {
            var identity = new CommandIdentity(client.ConnectionString, sql, null, null, typeof(T));
            var cache = CommandCache.Get(identity);
            if (cache != null)
                return await IxForceGetAsync(client, (IMapper<T>)cache.Mapper, sql, param);

            var dbParam = GenerateParameters(client, param).ToList();
            var rows = await IxRowsAsync(client, sql, dbParam);

            IDataRecord row;
            int rowCount;
            if (!DbClientHelper.TryReadUniqueRow(rows, out row, out rowCount))
                throw new IncorrectResultSizeException(sql, CommandType.Text, dbParam, 1, rowCount);

            var mapper = MapperParser.Parse<T>(row);
            CommandCache.Set(identity, new CommandCacheItem { Mapper = mapper });

            var res = mapper.MapRow(row, 1);
            return res;
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例集合，由一个模板对象指定目标类型。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="template">用于指定目标类型的模板对象。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static async Task<IList<T>> IxTemplateListAsync<T>(this IDbClient client, T template, string sql, params object[] param)
        {
            return (await IxQueryAsync<T>(client, sql, param)).ToList();
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例集合。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static async Task<IList<T>> IxListAsync<T>(this IDbClient client, string sql, params object[] param)
        {
            return (await IxQueryAsync<T>(client, sql, param)).ToList();
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例序列，由一个模板对象指定目标类型。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="template">用于指定目标类型的模板对象。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static Task<IEnumerable<T>> IxTemplateQueryAsync<T>(
            this IDbClient client, T template, string sql, params object[] param)
        {
            return IxQueryAsync<T>(client, sql, param);
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例序列。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">参数表。</param>
        /// <returns>目标类型的实例序列。</returns>
        public static async Task<IEnumerable<T>> IxQueryAsync<T>(this IDbClient client, string sql, params object[] param)
        {
            var identity = new CommandIdentity(client.ConnectionString, sql, null, null, typeof(T));
            var cache = CommandCache.Get(identity);
            var mapper = (IMapper<T>)cache?.Mapper;

            var rows = await IxRowsAsync(client, sql, param);

            return rows.Select((row, idx) =>
            {
                // mapper没有缓存时，在第一行上创建mapper并加入缓存。
                if (mapper == null)
                {
                    mapper = MapperParser.Parse<T>(row);
                    CommandCache.Set(identity, new CommandCacheItem { Mapper = mapper });
                }

                return mapper.MapRow(row, idx + 1);
            });
        }
    }
}
