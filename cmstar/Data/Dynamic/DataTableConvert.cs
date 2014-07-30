using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

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

            if (dataTable.Rows.Count == 0)
                return new T[0];

            var id = new DataTableIdentity(typeof(T), dataTable);
            var recordAdapter = new DataRecordFromDataTable(dataTable);

            object mapperObj;
            if (!DataTableMappers.TryGetValue(id, out mapperObj))
            {
                mapperObj = MapperParser.Parse<T>(recordAdapter);
                DataTableMappers[id] = mapperObj;
            }

            var mapper = (IMapper<T>)mapperObj;
            var rowCount = dataTable.Rows.Count;
            var resultList = new List<T>(rowCount);

            for (int i = 0; i < rowCount; i++)
            {
                recordAdapter.RowIndex = i;

                var t = mapper.MapRow(recordAdapter, i + 1);
                resultList.Add(t);
            }

            return resultList;
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

        private class DataRecordFromDataTable : IDataRecord
        {
            private readonly DataTable _table;
            private DataRow _currentRow;

            public DataRecordFromDataTable(DataTable dataTable)
            {
                _table = dataTable;
                _currentRow = _table.Rows[0];
            }

            public int RowIndex
            {
                set { _currentRow = _table.Rows[value]; }
            }

            public int FieldCount
            {
                get { return _table.Columns.Count; }
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new InvalidOperationException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new InvalidOperationException();
            }

            public IDataReader GetData(int i)
            {
                throw new InvalidOperationException();
            }

            public string GetDataTypeName(int i)
            {
                throw new InvalidOperationException();
            }

            public Type GetFieldType(int i)
            {
                return _table.Columns[i].DataType;
            }

            public string GetName(int i)
            {
                return _table.Columns[i].ColumnName;
            }

            public int GetOrdinal(string name)
            {
                return _table.Columns["name"].Ordinal;
            }

            public object GetValue(int i)
            {
                return _currentRow[i];
            }

            public int GetValues(object[] values)
            {
                throw new InvalidOperationException();
            }

            public object this[string name]
            {
                get { return _currentRow[name]; }
            }

            public object this[int i]
            {
                get { return GetValue(i); }
            }

            public bool GetBoolean(int i)
            {
                return Convert.ToBoolean(GetValue(i));
            }

            public byte GetByte(int i)
            {
                return Convert.ToByte(GetValue(i));
            }

            public char GetChar(int i)
            {
                return Convert.ToChar(GetValue(i));
            }

            public Guid GetGuid(int i)
            {
                var value = GetValue(i);

                if (value is SqlGuid)
                    return ((SqlGuid)value).Value;

                if (value is Guid)
                    return (Guid)value;

                return new Guid(value.ToString());
            }

            public short GetInt16(int i)
            {
                return Convert.ToInt16(GetValue(i));
            }

            public int GetInt32(int i)
            {
                return Convert.ToInt32(GetValue(i));
            }

            public long GetInt64(int i)
            {
                return Convert.ToInt64(GetValue(i));
            }

            public float GetFloat(int i)
            {
                return Convert.ToSingle(GetValue(i));
            }

            public double GetDouble(int i)
            {
                return Convert.ToDouble(GetValue(i));
            }

            public string GetString(int i)
            {
                return GetValue(i).ToString();
            }

            public decimal GetDecimal(int i)
            {
                return Convert.ToDecimal(GetValue(i));
            }

            public DateTime GetDateTime(int i)
            {
                return Convert.ToDateTime(GetValue(i));
            }

            public bool IsDBNull(int i)
            {
                return GetValue(i) is DBNull;
            }
        }
    }
}
