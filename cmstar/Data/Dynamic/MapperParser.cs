using System;
using System.Data;

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

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return (IMapper<T>)ParseNullableTypeMapper<T>(template);

            if (typeof(IConvertible).IsAssignableFrom(type))
                return Mappers.Convertible<T>();

            return new ObjectMapper<T>(template);
        }

        private static object ParseNullableTypeMapper<T>(IDataRecord template)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeof(T));
            object underlyingMapper;

            if (typeof(IConvertible).IsAssignableFrom(underlyingType))
            {
                var method = typeof(Mappers).GetMethod("Convertible", new[] { typeof(string) });
                var genericMethod = method.MakeGenericMethod(new[] { underlyingType });
                underlyingMapper = genericMethod.Invoke(null, new object[] { null });
            }
            else
            {
                var underlyingMapperType = typeof(ObjectMapper<>).MakeGenericType(underlyingType);
                underlyingMapper = Activator.CreateInstance(underlyingMapperType, template);
            }

            var nullableTypeMapper = Activator.CreateInstance(
                typeof(NullableTypeMapper<,>).MakeGenericType(new[] { typeof(T), underlyingType }),
                underlyingMapper);

            return nullableTypeMapper;
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
    }
}
