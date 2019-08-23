using System;
using System.Data;
using System.Diagnostics;
using cmstar.Data.Reflection;
using cmstar.Data.Reflection.Emit;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// Provides methods for creating <see cref="IMapper{T}"/>s from a template.
    /// </summary>
    internal static class MapperParser
    {
        public static IMapper<T> Parse<T>(IDataRecord template)
        {
            var type = typeof(T);
            var fieldType = template.GetFieldType(0);

            if (type.IsAssignableFrom(fieldType))
                return Mappers.Direct<T>();

            if (ReflectionUtils.IsAnonymousType(type))
                return new AnonymousObjectMapper<T>(template);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return (IMapper<T>)ParseNullableTypeMapper<T>(template);

            if (type.IsEnum)
                return EnumMapper<T>.Instance;

            if (typeof(IConvertible).IsAssignableFrom(type))
                return Mappers.Convertible<T>();

            return new ObjectMapper<T>(template);
        }

        private static object ParseNullableTypeMapper<T>(IDataRecord template)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeof(T));
            Debug.Assert(underlyingType != null, nameof(underlyingType) + " != null");

            object valueMapper;
            if (underlyingType.IsEnum)
            {
                var underlyingMapperType = typeof(EnumMapper<>).MakeGenericType(underlyingType);
                var field = underlyingMapperType.GetField(nameof(EnumMapper<int>.Instance));
                valueMapper = field.GetValue(null);
            }
            else if (typeof(IConvertible).IsAssignableFrom(underlyingType))
            {
                valueMapper = CreateConvertibleTypeMapper(underlyingType);
            }
            else
            {
                var underlyingMapperType = typeof(ObjectMapper<>).MakeGenericType(underlyingType);
                valueMapper = Activator.CreateInstance(underlyingMapperType, template);
            }

            var nullableTypeMapper = Activator.CreateInstance(
                typeof(NullableTypeMapper<,>).MakeGenericType(new[] { typeof(T), underlyingType }),
                valueMapper);

            return nullableTypeMapper;
        }

        private static object CreateConvertibleTypeMapper(Type type)
        {
            var method = typeof(Mappers).GetMethod(nameof(Mappers.Convertible), new[] { typeof(string) });
            Debug.Assert(method != null, nameof(method) + " != null");

            var genericMethod = method.MakeGenericMethod(type);
            var mapper = genericMethod.Invoke(null, new object[] { null });
            return mapper;
        }

        private class NullableTypeMapper<TNullable, TUnderlying> : IMapper<TNullable>
        {
            private readonly IMapper<TUnderlying> _underlyingMapper;

            public NullableTypeMapper(IMapper<TUnderlying> underlyingMapper)
            {
                _underlyingMapper = underlyingMapper;
            }

            public TNullable MapRow(IDataRecord record, int rowNum)
            {
                var v = record[0];

                if (v == null || v == DBNull.Value)
                {
                    return default(TNullable);
                }

                // can not avoid box & unbox here...
                object res = _underlyingMapper.MapRow(record, rowNum);
                return (TNullable)res;
            }
        }

        private class EnumMapper<T> : IMapper<T>
        {
            public static readonly EnumMapper<T> Instance = new EnumMapper<T>();

            private readonly object _underlyingTypeMapper;
            private readonly Func<object, object[], object> _mapRowCaller;

            private EnumMapper()
            {
                var underlyingType = Enum.GetUnderlyingType(typeof(T));
                _underlyingTypeMapper = CreateConvertibleTypeMapper(underlyingType);

                var mapRowMethod = _underlyingTypeMapper.GetType().GetMethod(nameof(IMapper<int>.MapRow));
                Debug.Assert(mapRowMethod != null, nameof(mapRowMethod) + " != null");

                _mapRowCaller = MethodInvokerGenerator.CreateDelegate(mapRowMethod);
            }

            public T MapRow(IDataRecord record, int rowNum)
            {
                var index = _mapRowCaller(_underlyingTypeMapper, new object[] { record, rowNum });
                var value = Enum.ToObject(typeof(T), index);
                return (T)value;
            }
        }
    }
}
