using System;
using System.Collections.Generic;
using System.Data;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// Provides methods for converting CLR types to DbTypes.
    /// </summary>
    internal class DbTypeConvert
    {
        /// <summary>
        /// This value will be used for DbParameter.Size when the DbParameter.Value is string 
        /// and the length of the string is not greater than this value.
        /// </summary>
        public const int DefaultStringSizeForDbParameter = 4000;

        /// <summary>
        /// A special value represents that a CLR type has no corresponding <see cref="DbType"/>.
        /// </summary>
        public const DbType NotSupporteDbType = (DbType)(-1);

        private const string LinqBinary = "System.Data.Linq.Binary";
        private static readonly Dictionary<Type, DbType> TypeMap;

        static DbTypeConvert()
        {
            TypeMap = new Dictionary<Type, DbType>();
            TypeMap[typeof(byte)] = DbType.Byte;
            TypeMap[typeof(sbyte)] = DbType.SByte;
            TypeMap[typeof(short)] = DbType.Int16;
            TypeMap[typeof(ushort)] = DbType.UInt16;
            TypeMap[typeof(int)] = DbType.Int32;
            TypeMap[typeof(uint)] = DbType.UInt32;
            TypeMap[typeof(long)] = DbType.Int64;
            TypeMap[typeof(ulong)] = DbType.UInt64;
            TypeMap[typeof(float)] = DbType.Single;
            TypeMap[typeof(double)] = DbType.Double;
            TypeMap[typeof(decimal)] = DbType.Decimal;
            TypeMap[typeof(bool)] = DbType.Boolean;
            TypeMap[typeof(string)] = DbType.String;
            TypeMap[typeof(char)] = DbType.StringFixedLength;
            TypeMap[typeof(Guid)] = DbType.Guid;
            TypeMap[typeof(DateTime)] = DbType.DateTime;
            TypeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            TypeMap[typeof(TimeSpan)] = DbType.Time;
            TypeMap[typeof(byte[])] = DbType.Binary;
            TypeMap[typeof(byte?)] = DbType.Byte;
            TypeMap[typeof(sbyte?)] = DbType.SByte;
            TypeMap[typeof(short?)] = DbType.Int16;
            TypeMap[typeof(ushort?)] = DbType.UInt16;
            TypeMap[typeof(int?)] = DbType.Int32;
            TypeMap[typeof(uint?)] = DbType.UInt32;
            TypeMap[typeof(long?)] = DbType.Int64;
            TypeMap[typeof(ulong?)] = DbType.UInt64;
            TypeMap[typeof(float?)] = DbType.Single;
            TypeMap[typeof(double?)] = DbType.Double;
            TypeMap[typeof(decimal?)] = DbType.Decimal;
            TypeMap[typeof(bool?)] = DbType.Boolean;
            TypeMap[typeof(char?)] = DbType.StringFixedLength;
            TypeMap[typeof(Guid?)] = DbType.Guid;
            TypeMap[typeof(DateTime?)] = DbType.DateTime;
            TypeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            TypeMap[typeof(TimeSpan?)] = DbType.Time;
            TypeMap[typeof(Object)] = DbType.Object;
        }

        /// <summary>
        /// Get the corresponding <see cref="DbType"/> of the specified CLR type.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <returns>The <see cref="DbType"/>.</returns>
        public static DbType LookupDbType(Type type)
        {
            var nullUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullUnderlyingType != null)
                type = nullUnderlyingType;

            if (type.IsEnum && !TypeMap.ContainsKey(type))
                type = Enum.GetUnderlyingType(type);

            DbType dbType;
            if (TypeMap.TryGetValue(type, out dbType))
                return dbType;

            if (type.FullName == LinqBinary)
                return DbType.Binary;

            return NotSupporteDbType;
        }
    }
}
