using System;
using System.Data;
using System.Data.SqlTypes;

namespace cmstar.Data
{
    public class DataRowRecord : IDataRecord
    {
        private readonly DataTable _table;
        private readonly DataRow _currentRow;

        public DataRowRecord(DataRow dataRow)
        {
            if (dataRow == null)
                throw new ArgumentNullException("dataRow");

            _currentRow = dataRow;
            _table = dataRow.Table;
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
            return GetFieldType(i).Name;
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
            return _table.Columns[name].Ordinal;
        }

        public object GetValue(int i)
        {
            return _currentRow[i];
        }

        public int GetValues(object[] values)
        {
            ArgAssert.NotNull(values, "values");

            var itemArray = _currentRow.ItemArray;
            var len = Math.Min(itemArray.Length, values.Length);

            Array.Copy(itemArray, values, len);
            return len;
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
