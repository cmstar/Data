using System;
using System.Data;

namespace cmstar.Data
{
    /// <summary>
    /// 定义了创建常见类型的<see cref="IMapper{T}"/>实现的有关方法。
    /// </summary>
    public static class Mappers
    {
        /// <summary>
        /// 通过委托定义创建<see cref="IMapper{T}"/>实现。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="rowMapping">从<see cref="IDataRecord"/>获取目标类型的数据的方法。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="rowMapping"/>为null。</exception>
        public static IMapper<T> Custom<T>(Func<IDataRecord, int, T> rowMapping)
        {
            ArgAssert.NotNull(rowMapping, "rowMapping");
            return new FunctionMapper<T>(rowMapping);
        }

        /// <summary>
        /// 创建映射字符串字段值的<see cref="IMapper{T}"/>实现。
        /// </summary>
        /// <param name="colName">使用的列名称，若为null则使用第一列。</param>
        /// <param name="allowNull"><c>true</c>则<see cref="DBNull"/>将被转换为空字符串；否则转换为null。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        public static IMapper<string> String(string colName = null, bool allowNull = false)
        {
            return new StringMapperImpl(colName, allowNull);
        }

        /// <summary>
        /// 创建映射<see cref="System.Int32"/>字段值的<see cref="IMapper{T}"/>实现。
        /// </summary>
        /// <param name="colName">使用的列名称，若为null则使用第一列。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        public static IMapper<int> Int32(string colName = null)
        {
            return new Int32MapperImpl(colName);
        }

        /// <summary>
        /// 创建映射<see cref="System.Int64"/>字段值的<see cref="IMapper{T}"/>实现。
        /// </summary>
        /// <param name="colName">使用的列名称，若为null则使用第一列。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        public static IMapper<long> Int64(string colName = null)
        {
            return new Int64MapperImpl(colName);
        }

        /// <summary>
        /// 创建映射<see cref="System.Decimal"/>字段值的<see cref="IMapper{T}"/>实现。
        /// </summary>
        /// <param name="colName">使用的列名称，若为null则使用第一列。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        public static IMapper<decimal> Decimal(string colName = null)
        {
            return new DecimalMapperImpl(colName);
        }

        /// <summary>
        /// 创建映射<see cref="System.Double"/>字段值的<see cref="IMapper{T}"/>实现。
        /// </summary>
        /// <param name="colName">使用的列名称，若为null则使用第一列。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        public static IMapper<double> Double(string colName = null)
        {
            return new DoubleMapperImpl(colName);
        }

        /// <summary>
        /// 直接返回指定类型，并不进行类型转换的<see cref="IMapper{T}"/>实现。
        /// </summary>
        /// <typeparam name="T">指定类型。</typeparam>
        /// <param name="colName">使用的列名称，若为null则使用第一列。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        public static IMapper<T> Direct<T>(string colName = null)
        {
            return new DirectTypeMapperImpl<T>(colName);
        }

        /// <summary>
        /// 创建对应指定类型的<see cref="IMapper{T}"/>实现，
        /// 指定的类型必须实现<see cref="IConvertible"/>。
        /// </summary>
        /// <typeparam name="T">指定类型。</typeparam>
        /// <param name="colName">使用的列名称，若为null则使用第一列。</param>
        /// <returns><see cref="IMapper{T}"/>的实例。</returns>
        /// <exception cref="InvalidCastException">
        /// 当类型<typeparamref name="T"/>未实现<see cref="IConvertible"/>。
        /// </exception>
        public static IMapper<T> Convertible<T>(string colName = null)
        {
            var t = typeof(T);
            if (!typeof(IConvertible).IsAssignableFrom(t))
            {
                var msg = string.Format("The type {0} does not implement IConvertible.", t.FullName);
                throw new InvalidCastException(msg);
            }

            // 对已实现的类型返回对应的mapper以减少装箱操作
            if (t == typeof(string))
                return (IMapper<T>)String(colName);

            if (t == typeof(int))
                return (IMapper<T>)Int32(colName);

            if (t == typeof(long))
                return (IMapper<T>)Int64(colName);

            if (t == typeof(double))
                return (IMapper<T>)Double(colName);

            if (t == typeof(decimal))
                return (IMapper<T>)Decimal(colName);

            // 对其他类型返回通用mapper
            return new ConvertibleTypeMapperImpl<T>(colName);
        }

        private class StringMapperImpl : IMapper<string>
        {
            private readonly string _colName;
            private readonly bool _allowNull;

            public StringMapperImpl(string colName, bool allowNull)
            {
                _colName = colName;
                _allowNull = allowNull;
            }

            public string MapRow(IDataRecord record, int rowNum)
            {
                var v = _colName == null ? record[0] : record[_colName];
                return _allowNull && v is DBNull ? null : v.ToString();
            }
        }

        private class FunctionMapper<T> : IMapper<T>
        {
            private readonly Func<IDataRecord, int, T> _rowMapping;

            public FunctionMapper(Func<IDataRecord, int, T> rowMapping)
            {
                _rowMapping = rowMapping;
            }

            public T MapRow(IDataRecord record, int rowNum)
            {
                return _rowMapping(record, rowNum);
            }
        }

        private class Int32MapperImpl : IMapper<int>
        {
            private readonly string _colName;

            public Int32MapperImpl(string colName)
            {
                _colName = colName;
            }

            public int MapRow(IDataRecord record, int rowNum)
            {
                return _colName == null ? Convert.ToInt32(record[0]) : Convert.ToInt32(record[_colName]);
            }
        }

        private class Int64MapperImpl : IMapper<long>
        {
            private readonly string _colName;

            public Int64MapperImpl(string colName)
            {
                _colName = colName;
            }

            public long MapRow(IDataRecord record, int rowNum)
            {
                return _colName == null ? Convert.ToInt32(record[0]) : Convert.ToInt32(record[_colName]);
            }
        }

        private class DecimalMapperImpl : IMapper<decimal>
        {
            private readonly string _colName;

            public DecimalMapperImpl(string colName)
            {
                _colName = colName;
            }

            public decimal MapRow(IDataRecord record, int rowNum)
            {
                return _colName == null ? Convert.ToInt32(record[0]) : Convert.ToInt32(record[_colName]);
            }
        }

        private class DoubleMapperImpl : IMapper<double>
        {
            private readonly string _colName;

            public DoubleMapperImpl(string colName)
            {
                _colName = colName;
            }

            public double MapRow(IDataRecord record, int rowNum)
            {
                return _colName == null ? Convert.ToInt32(record[0]) : Convert.ToInt32(record[_colName]);
            }
        }

        private class ConvertibleTypeMapperImpl<T> : IMapper<T>
        {
            private readonly string _colName;

            public ConvertibleTypeMapperImpl(string colName = null)
            {
                _colName = colName;
            }

            public T MapRow(IDataRecord record, int rowNum)
            {
                var v = _colName == null ? record[0] : record[_colName];
                return (T)Convert.ChangeType(v, typeof(T));
            }
        }

        private class DirectTypeMapperImpl<T> : IMapper<T>
        {
            private readonly string _colName;

            public DirectTypeMapperImpl(string colName = null)
            {
                _colName = colName;
            }

            public T MapRow(IDataRecord record, int rowNum)
            {
                var v = _colName == null ? record[0] : record[_colName];
                return (T)v;
            }
        }
    }
}
