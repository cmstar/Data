using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace cmstar.Data
{
    /// <summary>
    /// 提供<see cref="IDataRecord"/>到<see cref="SingleRowKeeper"/>的映射。
    /// </summary>
    internal class SingleRowKeeperMapper : IMapper<SingleRowKeeper>
    {
        public SingleRowKeeper MapRow(IDataRecord record, int rowNum)
        {
            return new SingleRowKeeper(record);
        }
    }

    /// <summary>
    /// 提供<see cref="IDataReader"/>的第一行记录的存储与检索。
    /// </summary>
    internal class SingleRowKeeper : IDataRecord
    {
        private readonly object[] _values;
        private readonly string[] _columnNames;
        private Dictionary<string, int> _nameMap;

        /// <summary>
        /// 初始化<see cref="SingleRowKeeper"/>的新实例，并制定提供记录数据的<see cref="IDataRecord"/>。
        /// </summary>
        /// <param name="dataRecord">
        /// <see cref="IDataRecord"/>的实例，其第一行数据将被记录下来。
        /// </param>
        public SingleRowKeeper(IDataRecord dataRecord)
        {
            ArgAssert.NotNull(dataRecord, "dataRecord");

            FieldCount = dataRecord.FieldCount;

            //keep meta data
            _columnNames = new string[dataRecord.FieldCount];
            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                _columnNames[i] = dataRecord.GetName(i);
            }

            //keep data values
            _values = new object[dataRecord.FieldCount];
            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                _values[i] = dataRecord.GetValue(i);
            }
        }

        public int FieldCount { get; private set; }

        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        public object this[string name]
        {
            get
            {
                var index = GetOrdinal(name);
                return _values[index];
            }
        }

        public string GetName(int i)
        {
            ArgAssert.Between(i, "i", 0, _columnNames.Length);
            return _columnNames[i];
        }

        public object GetValue(int i)
        {
            ArgAssert.Between(i, "i", 0, _values.Length);
            return _values[i];
        }

        public int GetValues(object[] values)
        {
            ArgAssert.NotNull(values, "values");

            var num = Math.Min(values.Length, _values.Length);
            Array.Copy(_values, values, num);
            return num;
        }

        public int GetOrdinal(string name)
        {
            ArgAssert.NotNullOrEmpty(name, "name");

            //根据列的数量选择是遍历列，还是构造列的哈希表进行检索
            return _columnNames.Length > 8 ? GetOrdinalByNameMap(name) : GetOrdinalByIteration(name);
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

        string IDataRecord.GetDataTypeName(int i)
        {
            throw InvalidOperation();
        }

        Type IDataRecord.GetFieldType(int i)
        {
            throw InvalidOperation();
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw InvalidOperation();
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw InvalidOperation();
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw InvalidOperation();
        }

        private InvalidOperationException InvalidOperation()
        {
            throw new InvalidOperationException();
        }

        private int GetOrdinalByNameMap(string name)
        {
            if (_nameMap == null)
            {
                _nameMap = new Dictionary<string, int>(_columnNames.Length);
                for (int i = 0; i < _columnNames.Length; i++)
                {
                    _nameMap[_columnNames[i]] = i;
                }
            }

            int index;
            if (!_nameMap.TryGetValue(name, out index))
                throw new IndexOutOfRangeException(name);

            return index;
        }

        private int GetOrdinalByIteration(string name)
        {
            for (int i = 0; i < _columnNames.Length; i++)
            {
                if (name == _columnNames[i])
                    return i;
            }

            throw new IndexOutOfRangeException(name);
        }
    }
}
