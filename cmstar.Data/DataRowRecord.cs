using System;
using System.Data;

namespace cmstar.Data
{
    /// <summary>
    /// 提供<see cref="DataRow"/>到<see cref="IDataRecord"/>的转换适配。
    /// </summary>
    public class DataRowRecord : AbstractDataRecord
    {
        private readonly DataTable _table;
        private readonly DataRow _currentRow;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="dataRow">作为数据源的<see cref="DataRow"/>。</param>
        public DataRowRecord(DataRow dataRow)
        {
            ArgAssert.NotNull(dataRow, "dataRow");

            _currentRow = dataRow;
            _table = dataRow.Table;
        }

        public override int FieldCount
        {
            get { return _table.Columns.Count; }
        }

        public override Type GetFieldType(int i)
        {
            return _table.Columns[i].DataType;
        }

        public override object GetValue(int i)
        {
            return _currentRow[i];
        }

        public override string GetName(int i)
        {
            return _table.Columns[i].ColumnName;
        }

        public override int GetOrdinal(string name)
        {
            return _table.Columns[name].Ordinal;
        }

        public override int GetValues(object[] values)
        {
            ArgAssert.NotNull(values, "values");

            var itemArray = _currentRow.ItemArray;
            var len = Math.Min(itemArray.Length, values.Length);

            Array.Copy(itemArray, values, len);
            return len;
        }
    }
}
