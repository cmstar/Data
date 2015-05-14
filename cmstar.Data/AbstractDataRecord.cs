using System;
using System.Data;
using System.Data.SqlTypes;

namespace cmstar.Data
{
    /// <summary>
    /// 提供<see cref="IDataRecord"/>的一个抽象基类。可以继承此类型进行<see cref="IDataRecord"/>的简易实现。
    /// </summary>
    public abstract class AbstractDataRecord : IDataRecord
    {
        public abstract int FieldCount { get; }

        public virtual string GetDataTypeName(int i)
        {
            return GetFieldType(i).Name;
        }

        public abstract Type GetFieldType(int i);

        public abstract string GetName(int i);

        public abstract int GetOrdinal(string name);

        public abstract object GetValue(int i);

        public virtual int GetValues(object[] values)
        {
            ArgAssert.NotNull(values, "values");

            var num = Math.Min(values.Length, FieldCount);
            for (int i = 0; i <= num; i++)
            {
                values[i] = GetValue(i);
            }

            return num;
        }

        public virtual object this[string name]
        {
            get
            {
                var index = GetOrdinal(name);
                return GetValue(index);
            }
        }

        public virtual object this[int i]
        {
            get { return GetValue(i); }
        }

        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new InvalidOperationException();
        }

        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new InvalidOperationException();
        }

        public virtual IDataReader GetData(int i)
        {
            throw new InvalidOperationException();
        }

        public virtual bool GetBoolean(int i)
        {
            return Convert.ToBoolean(GetValue(i));
        }

        public virtual byte GetByte(int i)
        {
            return Convert.ToByte(GetValue(i));
        }

        public virtual char GetChar(int i)
        {
            return Convert.ToChar(GetValue(i));
        }

        public virtual Guid GetGuid(int i)
        {
            var value = GetValue(i);
            if (value == null)
                throw new InvalidCastException();

            if (value is SqlGuid)
                return ((SqlGuid)value).Value;

            if (value is Guid)
                return (Guid)value;

            return new Guid(value.ToString());
        }

        public virtual short GetInt16(int i)
        {
            return Convert.ToInt16(GetValue(i));
        }

        public virtual int GetInt32(int i)
        {
            return Convert.ToInt32(GetValue(i));
        }

        public virtual long GetInt64(int i)
        {
            return Convert.ToInt64(GetValue(i));
        }

        public virtual float GetFloat(int i)
        {
            return Convert.ToSingle(GetValue(i));
        }

        public virtual double GetDouble(int i)
        {
            return Convert.ToDouble(GetValue(i));
        }

        public virtual string GetString(int i)
        {
            return GetValue(i).ToString();
        }

        public virtual decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(GetValue(i));
        }

        public virtual DateTime GetDateTime(int i)
        {
            return Convert.ToDateTime(GetValue(i));
        }

        public virtual bool IsDBNull(int i)
        {
            return GetValue(i) is DBNull;
        }
    }
}
