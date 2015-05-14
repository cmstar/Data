using System;
using System.Collections.Generic;
using System.Data;

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
    internal class SingleRowKeeper : AbstractDataRecord
    {
        private readonly int _fieldCount;
        private readonly object[] _values;
        private readonly Type[] _fieldTypes;
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

            var fieldCount = dataRecord.FieldCount;
            _fieldCount = fieldCount;

            // keep meta data & values
            _values = new object[fieldCount];
            _fieldTypes = new Type[fieldCount];
            _columnNames = new string[fieldCount];

            for (int i = 0; i < fieldCount; i++)
            {
                _values[i] = dataRecord.GetValue(i);
                _fieldTypes[i] = dataRecord.GetFieldType(i);
                _columnNames[i] = dataRecord.GetName(i);
            }
        }

        public override int FieldCount
        {
            get { return _fieldCount; }
        }

        public override string GetName(int i)
        {
            ArgAssert.Between(i, "i", 0, _fieldCount);
            return _columnNames[i];
        }

        public override object GetValue(int i)
        {
            ArgAssert.Between(i, "i", 0, _fieldCount);
            return _values[i];
        }

        public override int GetValues(object[] values)
        {
            ArgAssert.NotNull(values, "values");

            var num = Math.Min(values.Length, _fieldCount);
            Array.Copy(_values, values, num);
            return num;
        }

        public override int GetOrdinal(string name)
        {
            ArgAssert.NotNullOrEmpty(name, "name");

            //根据列的数量选择是遍历列，还是构造列的哈希表进行检索
            return _fieldCount > 8 ? GetOrdinalByNameMap(name) : GetOrdinalByIteration(name);
        }

        public override Type GetFieldType(int i)
        {
            ArgAssert.Between(i, "i", 0, _fieldCount);
            return _fieldTypes[i];
        }

        private int GetOrdinalByNameMap(string name)
        {
            if (_nameMap == null)
            {
                _nameMap = new Dictionary<string, int>(_fieldCount);
                for (int i = 0; i < _fieldCount; i++)
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
            for (int i = 0; i < _fieldCount; i++)
            {
                if (name == _columnNames[i])
                    return i;
            }

            throw new IndexOutOfRangeException(name);
        }
    }
}
