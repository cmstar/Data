using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using cmstar.Data.Reflection.Emit;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// 提供<see cref="IDbClient"/>的扩展方法，这些方法可以将CLR对象用于SQL传参。
    /// </summary>
    public static class ObjectiveExtension
    {
        /// <summary>
        /// 获取查询的第一行第一列的值。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>查询结果的第一行第一列的值。若查询结果行数为0，返回<c>null</c>。</returns>
        public static object Scalar(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.Scalar(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 执行非查询SQL语句，并返回所影响的行数。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>SQL所影响的行数。</returns>
        public static int Execute(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.Execute(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 执行非查询SQL语句，并断言所影响的行数。若影响的函数不正确，抛出异常。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="expectedSize">被断言的影响行数。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <exception cref="IncorrectResultSizeException">当影响的行数不正确。</exception>
        public static void SizedExecute(this IDbClient client,
            int expectedSize, string sql, object param,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            client.SizedExecute(expectedSize, cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataTable"/>。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static DataTable DataTable(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.DataTable(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 返回查询语句对应查询结果的<see cref="System.Data.DataSet"/>。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>表示查询结果的<see cref="System.Data.DataTable"/>。</returns>
        public static DataSet DataSet(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.DataSet(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 判断给定的查询的结果是否至少包含1行。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>若查询结果至少包含1行，返回<c>true</c>；否则返回<c>false</c>。</returns>
        public static bool Exists(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.Exists(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 获取查询结果的第一行记录。
        /// 若查询命中的行数为0，返回null。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns><see cref="IDataRecord"/>的实现，包含查询的第一行记录。</returns>
        public static IDataRecord GetRow(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.GetRow(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 获取查询结果的第一行记录，以数组形式返回记录内各列的值。
        /// 数组元素顺序与列顺序一致。若查询命中的行数为0，返回null。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>包含了各列的值的数组。</returns>
        public static object[] ItemArray(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.ItemArray(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        public static T Get<T>(this IDbClient client,
            IMapper<T> mapper, string sql, object param,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.Get(mapper, cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象。
        /// SQL命中的记录必须为1行，否则抛出异常。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static T ForceGet<T>(this IDbClient client,
            IMapper<T> mapper, string sql, object param,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.ForceGet(mapper, cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 使用<see cref="IMapper{T}"/>查询指定对象的集合。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例的集合。若查询命中的行数为0，返回空集合。</returns>
        public static IList<T> List<T>(this IDbClient client,
            IMapper<T> mapper, string sql, object param,
            CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.List(mapper, cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 获取查询结果得行序列。
        /// </summary>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>查询结果得行序列。</returns>
        public static IEnumerable<IDataRecord> Rows(this IDbClient client,
            string sql, object param, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var cache = GetNoMapperCache(client, sql, param, commandType);
            return client.Rows(cache.Sql, cache.Params(client, param), commandType, timeOut);
        }

        /// <summary>
        /// 查询并根据结果创建目标类型的实例。
        /// 若满足条件的记录不存在，返回目标类型的默认值（对于引用类型为<c>null</c>）。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        public static T Get<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var paramType = param == null ? null : param.GetType();
            var id = new CommandIdentity(client.ConnectionString, sql, commandType, paramType, typeof(T));
            var cache = CommandCache.Get(id);

            if (cache != null)
            {
                return client.Get((IMapper<T>)cache.Mapper, cache.Sql,
                    cache.Params(client, param), commandType, timeOut);
            }

            cache = CreateCacheItem(sql, commandType, param);
            var rows = client.Rows(sql, cache.Params(client, param), commandType, timeOut);
            foreach (var row in rows)
            {
                var mapper = MapperParser.Parse<T>(row);
                cache.Mapper = mapper;
                CommandCache.Set(id, cache);
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
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例。</returns>
        /// <exception cref="IncorrectResultSizeException">当SQL命中的记录行数不为 1。</exception>
        public static T ForceGet<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var paramType = param == null ? null : param.GetType();
            var id = new CommandIdentity(client.ConnectionString, sql, commandType, paramType, typeof(T));
            var cache = CommandCache.Get(id);

            if (cache != null)
            {
                return client.ForceGet((IMapper<T>)cache.Mapper, cache.Sql,
                    cache.Params(client, param), commandType, timeOut);
            }

            cache = CreateCacheItem(sql, commandType, param);
            var dbParam = cache.Params(client, param);
            var rows = client.Rows(sql, dbParam, commandType, timeOut);
            var rowCount = 0;
            var res = default(T);

            using (var iter = rows.GetEnumerator())
            {
                if (iter.MoveNext())
                {
                    var mapper = MapperParser.Parse<T>(iter.Current);
                    cache.Mapper = mapper;
                    CommandCache.Set(id, cache);
                    res = mapper.MapRow(iter.Current, 1);
                    rowCount++;
                }

                while (iter.MoveNext())
                {
                    rowCount++;
                }
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
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static IList<T> TemplateList<T>(this IDbClient client,
            T template, string sql, object param = null, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return Query<T>(client, sql, param, commandType, timeOut).ToList();
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例集合。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例集合。</returns>
        public static IList<T> List<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return Query<T>(client, sql, param, commandType, timeOut).ToList();
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
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例序列。</returns>
        public static IEnumerable<T> TemplateQuery<T>(this IDbClient client,
            T template, string sql, object param = null, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            return Query<T>(client, sql, param, commandType, timeOut);
        }

        /// <summary>
        /// 查询并根据结果返回目标类型的实例序列。
        /// </summary>
        /// <typeparam name="T">查询的目标类型。</typeparam>
        /// <param name="client"><see cref="IDbClient"/>的实例。</param>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">记录SQL参数的对象。</param>
        /// <param name="commandType">命令的类型。</param>
        /// <param name="timeOut">命令的超时时间，单位毫秒。0为不指定。</param>
        /// <returns>目标类型的实例序列。</returns>
        public static IEnumerable<T> Query<T>(this IDbClient client,
            string sql, object param = null, CommandType commandType = CommandType.Text, int timeOut = 0)
        {
            var paramType = param == null ? null : param.GetType();
            var id = new CommandIdentity(client.ConnectionString, sql, commandType, paramType, typeof(T));
            var cache = CommandCache.Get(id);
            var mapper = cache == null ? null : (IMapper<T>)cache.Mapper;
            var rowNum = 0;

            if (cache == null)
            {
                cache = CreateCacheItem(sql, commandType, param);
            }

            var rows = client.Rows(sql, cache.Params(client, param), commandType, timeOut);

            // 展开foreach循环，使mapper == null的仅判断一次
            using (var iter = rows.GetEnumerator())
            {
                if (!iter.MoveNext())
                    yield break;

                if (mapper == null)
                {
                    mapper = MapperParser.Parse<T>(iter.Current);
                    cache.Mapper = mapper;
                    CommandCache.Set(id, cache);
                }

                do
                {
                    yield return mapper.MapRow(iter.Current, ++rowNum);

                } while (iter.MoveNext());
            }
        }

        // a parameter provider which just returns null
        private static IEnumerable<DbParameter> NullParamProvider(IDbClient client, object param)
        {
            return null;
        }

        private static CommandCacheItem GetNoMapperCache(IDbClient client, string sql, object param, CommandType commandType)
        {
            var id = new CommandIdentity(client.ConnectionString, sql, commandType, param.GetType(), null);
            var cache = CommandCache.Get(id);

            if (cache == null)
            {
                cache = CreateCacheItem(sql, commandType, param);
                CommandCache.Set(id, cache);
            }

            return cache;
        }

        private static CommandCacheItem CreateCacheItem(string sql, CommandType commandType, object param)
        {
            if (param == null || string.IsNullOrEmpty(sql))
                return new CommandCacheItem { Sql = sql, Params = NullParamProvider };

            // 参数可包含在属性或字段中，两种都要获取
            var paramType = param.GetType();
            var members = paramType
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Cast<MemberInfo>()
                .ToDictionary(x => x.Name, x => x);

            var props = paramType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                members.Add(prop.Name, prop);
            }

            var paramProvider = new ParamProvider();

            if (commandType == CommandType.Text)
            {
                var patter = "[@:][a-zA-Z0-9_]+";
                var matches = Regex.Matches(sql, patter);
                var existNames = new HashSet<string>();

                foreach (Match match in matches)
                {
                    var name = match.Value.Substring(1);

                    // 避免重复处理
                    if (!existNames.Add(name))
                        continue;

                    MemberInfo member;
                    if (!members.TryGetValue(name, out member))
                        continue;

                    paramProvider.AddDbParameterInfo(name, member);
                }
            }
            else
            {
                foreach (var member in members)
                {
                    paramProvider.AddDbParameterInfo(member.Key, member.Value);
                }
            }

            return new CommandCacheItem
            {
                Sql = sql,
                Params = paramProvider.GenerateParameters
            };
        }

        private class ParamProvider
        {
            private readonly List<DbParameterInfo> _parameterInfos = new List<DbParameterInfo>();

            public void AddDbParameterInfo(string paramName, MemberInfo memberInfo)
            {
                var isPropertyInfo = memberInfo is PropertyInfo;
                var memberType = isPropertyInfo
                    ? ((PropertyInfo)memberInfo).PropertyType
                    : ((FieldInfo)memberInfo).FieldType;
                var dbType = DbTypeConvert.LookupDbType(memberType);
                if (dbType == DbTypeConvert.NotSupporteDbType)
                {
                    throw new NotSupportedException(string.Format(
                        "The type {0} can not be converted to a DbType.", memberType));
                }

                var info = new DbParameterInfo();
                info.Name = paramName;
                info.DbType = dbType;
                info.IsString = memberType == typeof(string);
                info.ValueGetter = isPropertyInfo
                    ? PropertyAccessorGenerator.CreateGetter((PropertyInfo)memberInfo)
                    : FieldAccessorGenerator.CreateGetter((FieldInfo)memberInfo);

                _parameterInfos.Add(info);
            }

            public IEnumerable<DbParameter> GenerateParameters(IDbClient client, object param)
            {
                if (param == null)
                    yield break;

                foreach (var p in _parameterInfos)
                {
                    var dbParam = client.CreateParameter();
                    dbParam.ParameterName = p.Name;
                    dbParam.DbType = p.DbType;

                    var value = p.ValueGetter(param);
                    if (value == null)
                    {
                        dbParam.Value = DBNull.Value;
                    }
                    else
                    {
                        dbParam.Value = value;

                        if (p.IsString && ((string)value).Length <= DbTypeConvert.DefaultStringSizeForDbParameter)
                        {
                            dbParam.Size = DbTypeConvert.DefaultStringSizeForDbParameter;
                        }
                    }

                    yield return dbParam;
                }
            }
        }

        private class DbParameterInfo
        {
            public string Name;
            public DbType DbType;
            public bool IsString;
            public Func<object, object> ValueGetter;
        }
    }
}
