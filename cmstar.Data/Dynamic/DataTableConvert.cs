using System;
using System.Collections.Generic;
using System.Data;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// 包含将<see cref="DataTable"/>的行映射到指定对象的有关方法。
    /// </summary>
    public class DataTableConvert
    {
        private static readonly Dictionary<DataTableIdentity, object> DataTableMappers
            = new Dictionary<DataTableIdentity, object>();

        /// <summary>
        /// 从给定的<see cref="DataTable"/>的行创建特定类型的对象的集合。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="dataTable"><see cref="DataTable"/>的实例。</param>
        /// <returns>对象的集合。</returns>
        public static IList<T> To<T>(DataTable dataTable)
        {
            ArgAssert.NotNull(dataTable, "dataTable");

            var rowCount = dataTable.Rows.Count;
            if (dataTable.Rows.Count == 0)
                return new T[0];

            var id = new DataTableIdentity(typeof(T), dataTable);

            using (var reader = dataTable.CreateDataReader())
            {
                reader.Read();

                object mapperObj;
                if (!DataTableMappers.TryGetValue(id, out mapperObj))
                {
                    mapperObj = MapperParser.Parse<T>(reader);
                    DataTableMappers[id] = mapperObj;
                }

                var mapper = (IMapper<T>)mapperObj;
                var resultList = new List<T>(rowCount);
                var i = 1;

                do
                {
                    var t = mapper.MapRow(reader, i++);
                    resultList.Add(t);

                } while (reader.Read());

                return resultList;
            }
        }

        private class DataTableIdentity
        {
            private readonly string[] _columnNames;
            private readonly Type[] _columnTyps;
            private readonly Type _resultType;
            private readonly int _hashCode;

            public DataTableIdentity(Type resultType, DataTable dataTable)
            {
                var columnCount = dataTable.Columns.Count;

                _columnNames = new string[columnCount];
                _columnTyps = new Type[columnCount];
                _resultType = resultType;
                _hashCode = resultType.GetHashCode();

                for (int i = 0; i < columnCount; i++)
                {
                    var col = dataTable.Columns[i];

                    _columnNames[i] = col.ColumnName;
                    _columnTyps[i] = col.DataType;

                    unchecked
                    {
                        _hashCode = _hashCode * 23 + col.ColumnName.GetHashCode();
                        _hashCode = _hashCode * 23 + col.DataType.GetHashCode();
                    }
                }
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object obj)
            {
                var other = obj as DataTableIdentity;
                if (other == null)
                    return false;

                if (other._resultType != _resultType)
                    return false;

                if (other._columnNames.Length != _columnNames.Length)
                    return false;

                for (int i = 0; i < _columnNames.Length; i++)
                {
                    if (other._columnNames[i] != _columnNames[i])
                        return false;

                    if (other._columnTyps[i] != _columnTyps[i])
                        return false;
                }

                return true;
            }
        }
    }
}
