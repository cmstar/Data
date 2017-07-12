using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using cmstar.Data.Dynamic;

namespace cmstar.Data
{
    // 实现 ObjectiveExtension 扩展方法的异步版本
    public static partial class ObjectiveExtension
    {
        /// <summary>
        /// 获取查询的第一行第一列的值。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>查询结果的第一行第一列的值。若查询结果行数为0，返回<c>null</c>。</returns>
        public static async Task<object> ScalarAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.ScalarAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 执行非查询SQL语句，并返回所影响的行数。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>SQL所影响的行数。</returns>
        public static async Task<int> ExecuteAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.ExecuteAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 执行非查询SQL语句，并断言所影响的行数。若影响的函数不正确，抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="expectedSize">被断言的影响行数。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public static async Task SizedExecuteAsync(this IDbClient client,
            int expectedSize, string sql, object param,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            await client.SizedExecuteAsync(expectedSize, cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataTable"/>。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static async Task<DataTable> DataTableAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.DataTableAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataSet"/>。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static async Task<DataSet> DataSetAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.DataSetAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 判断给定的查询的结果是否至少包含1行。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>若查询结果至少包含1行，返回<c>true</c>；否则返回<c>false</c>。</returns>
        public static async Task<bool> ExistsAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.ExistsAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 获取查询结果的第一行记录。
        /// 若查询命中的行数为0，返回null。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns><see cref="IDataRecord"/>的实现，包含查询的第一行记录。</returns>
        public static async Task<IDataRecord> GetRowAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.GetRowAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 获取查询结果的第一行记录，以数组形式返回记录内各列的值。
        /// 数组元素顺序与列顺序一致。若查询命中的行数为0，返回null。
        /// 这是一个异步操作。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>包含了各列的值的数组。</returns>
        public static async Task<object[]> ItemArrayAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.ItemArrayAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        public static async Task<T> GetAsync<T>(this IDbClient client,
            IMapper<T> mapper, string sql, object param,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.GetAsync(mapper, cache.Sql, cache.Params(client, param), commandType, timeout);
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
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static async Task<T> ForceGetAsync<T>(this IDbClient client,
            IMapper<T> mapper, string sql, object param,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.ForceGetAsync(mapper, cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 获取查询结果得行序列。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>查询结果得行序列。</returns>
        public static async Task<IEnumerable<IDataRecord>> RowsAsync(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.RowsAsync(cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象的集合。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例的集合。若查询命中的行数为0，返回空集合。</returns>
        public static async Task<IList<T>> ListAsync<T>(this IDbClient client,
            IMapper<T> mapper, string sql, object param,
            CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return await client.ListAsync(mapper, cache.Sql, cache.Params(client, param), commandType, timeout);
        }

        /// <summary>
        /// 查询并根据结果创建目标类型的实例。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        public static async Task<T> GetAsync<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var paramType = param?.GetType();
            var id = new CommandIdentity(client.ConnectionString, sql, commandType, paramType, typeof(T));
            var cache = CommandCache.Get(id);

            if (cache != null)
            {
                return await client.GetAsync(
                    (IMapper<T>)cache.Mapper, cache.Sql, cache.Params(client, param), commandType, timeout);
            }

            cache = CreateCacheItem(sql, commandType, param);

            var row = await client.GetRowAsync(sql, cache.Params(client, param), commandType, timeout);
            if (row == null)
                return default(T);

            var mapper = MapperParser.Parse<T>(row);
            cache.Mapper = mapper;
            CommandCache.Set(id, cache);

            var res = mapper.MapRow(row, 1);
            return res;
        }

        /// <summary>
        /// 查询并根据结果创建目标类型的实例。
        /// SQL命中的记录必须为1行，否则抛出异常。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static async Task<T> ForceGetAsync<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var paramType = param?.GetType();
            var id = new CommandIdentity(client.ConnectionString, sql, commandType, paramType, typeof(T));
            var cache = CommandCache.Get(id);

            if (cache != null)
            {
                return await client.ForceGetAsync((IMapper<T>)cache.Mapper, cache.Sql,
                    cache.Params(client, param), commandType, timeout);
            }

            cache = CreateCacheItem(sql, commandType, param);
            var dbParam = cache.Params(client, param);
            var rows = await client.RowsAsync(sql, dbParam, commandType, timeout);

            IDataRecord row;
            int rowCount;
            if (!DbClientHelper.TryReadUniqueRow(rows, out row, out rowCount))
                throw new IncorrectResultSizeException(sql, CommandType.Text, dbParam, 1, rowCount);

            var mapper = MapperParser.Parse<T>(row);
            cache.Mapper = mapper;
            CommandCache.Set(id, cache);

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
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static async Task<IList<T>> TemplateListAsync<T>(this IDbClient client,
            T template, string sql, object param = null, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return (await QueryAsync<T>(client, sql, param, commandType, timeout)).ToList();
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例集合。
        /// 这是一个异步操作。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static async Task<IList<T>> ListAsync<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return (await QueryAsync<T>(client, sql, param, commandType, timeout)).ToList();
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例序列，由一个模板对象指定目标类型。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="template">用于指定目标类型的模板对象。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例序列。</returns>
        public static async Task<IEnumerable<T>> TemplateQueryAsync<T>(this IDbClient client,
            T template, string sql, object param = null, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            return await QueryAsync<T>(client, sql, param, commandType, timeout);
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例序列。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeout">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例序列。</returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeout = 0)
        {
            var paramType = param?.GetType();
            var id = new CommandIdentity(client.ConnectionString, sql, commandType, paramType, typeof(T));
            var cache = CommandCache.Get(id);
            var mapper = (IMapper<T>)cache?.Mapper;

            if (cache == null)
            {
                cache = CreateCacheItem(sql, commandType, param);
            }

            var rows = await client.RowsAsync(sql, cache.Params(client, param), commandType, timeout);

            return rows.Select((row, idx) =>
            {
                // mapper没有缓存时，在第一行上创建mapper并加入缓存。
                if (mapper == null)
                {
                    mapper = MapperParser.Parse<T>(row);
                    cache.Mapper = mapper;
                    CommandCache.Set(id, cache);
                }

                return mapper.MapRow(row, idx + 1);
            });
        }
    }
}
