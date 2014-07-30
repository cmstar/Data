using System;
using System.Collections.Generic;
using System.Data;
using cmstar.RapidReflection.Emit;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// A mapper that maps data from a <see cref="IDataRecord"/> to
    /// anonymouse objects with the specified type.
    /// </summary>
    /// <typeparam name="T">The type which the data is mapped to.</typeparam>
    public class AnonymousObjectMapper<T> : IMapper<T>
    {
        private readonly Func<object[], object> _contructor;
        private readonly ArgumentInfo[] _argumentInfos;
        private readonly int _argumentLength;

        /// <summary>
        /// Initialize a new instance of <see cref="AnonymousObjectMapper{T}"/> with
        /// the specified <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="template">The template for the mapping.</param>
        public AnonymousObjectMapper(IDataRecord template)
        {
            var contructor = typeof(T).GetConstructors()[0];
            _contructor = ConstructorInvokerGenerator.CreateDelegate(contructor);

            var fieldMap = GetFieldMap(template);
            var parameters = contructor.GetParameters();

            _argumentLength = parameters.Length;
            _argumentInfos = new ArgumentInfo[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                var pType = p.ParameterType;
                var argumentInfo = new ArgumentInfo();
                argumentInfo.ParamType = pType;

                if (pType.IsValueType)
                {
                    argumentInfo.DefaultValue = Activator.CreateInstance(pType);
                }

                int index;
                if (fieldMap.TryGetValue(p.Name, out index))
                {
                    argumentInfo.DataColIndex = index;

                    var dataFieldType = template.GetFieldType(index);
                    argumentInfo.NeedConvertType = !pType.IsAssignableFrom(dataFieldType);
                }
                else
                {
                    argumentInfo.DataColIndex = -1;
                }

                _argumentInfos[i] = argumentInfo;
            }
        }

        public T MapRow(IDataRecord record, int rowNum)
        {
            var args = new object[_argumentLength];

            for (int i = 0; i < _argumentLength; i++)
            {
                var argumentInfo = _argumentInfos[i];

                object value;
                if (argumentInfo.DataColIndex < 0)
                {
                    value = argumentInfo.DefaultValue;
                }
                else
                {
                    value = record[argumentInfo.DataColIndex];

                    if (argumentInfo.NeedConvertType)
                        value = Convert.ChangeType(value, argumentInfo.ParamType);
                }

                args[i] = value;
            }

            var instance = _contructor(args);
            return (T)instance;
        }

        private Dictionary<string, int> GetFieldMap(IDataRecord dataRecord)
        {
            var map = new Dictionary<string, int>(dataRecord.FieldCount);

            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                var name = dataRecord.GetName(i);
                map[name] = i;
            }

            return map;
        }

        private struct ArgumentInfo
        {
            public int DataColIndex;
            public Type ParamType;
            public object DefaultValue;
            public bool NeedConvertType;
        }
    }
}
