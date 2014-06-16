using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using cmstar.RapidReflection.Emit;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// A mapper that maps data from a <see cref="IDataRecord"/> to
    /// the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type which the data is mapped to.</typeparam>
    internal class ObjectMapper<T> : IMapper<T>
    {
        private readonly Func<object> _targetConstructor;
        private readonly IList<MemberSetupInfo> _targetMemberSetups;

        /// <summary>
        /// Initialize a new instance of <see cref="ObjectMapper{T}"/> with
        /// the specified <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="template">
        /// The template for the mapping. The value setters will be initialized
        /// according to the template.
        /// </param>
        public ObjectMapper(IDataRecord template)
        {
            var type = typeof(T);

            _targetConstructor = ConstructorInvokerGenerator.CreateDelegate(type);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite)
                .ToDictionary(x => x.Name, x => x);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(x => x.Name, x => x);

            _targetMemberSetups = new List<MemberSetupInfo>();

            for (int i = 0; i < template.FieldCount; i++)
            {
                var name = template.GetName(i);

                if (properties.ContainsKey(name))
                {
                    var setupInfo = BuildSetupInfo(i, properties[name]);
                    _targetMemberSetups.Add(setupInfo);
                }
                else if (fields.ContainsKey(name))
                {
                    var setupInfo = BuildSetupInfo(i, fields[name]);
                    _targetMemberSetups.Add(setupInfo);
                }
            }
        }

        public T MapRow(IDataRecord record, int rowNum)
        {
            // don't cast obj to T here, if T is value type, a box is needed
            // since the value type will be clone into a box wrapper during boxing
            var obj = _targetConstructor();

            foreach (var setup in _targetMemberSetups)
            {
                var value = record.GetValue(setup.Index);

                if (value == null)
                    continue;

                try
                {
                    if (value.GetType() != setup.MemberType)
                        value = Convert.ChangeType(value, setup.MemberType);

                    setup.Setter(obj, value);
                }
                catch (Exception e)
                {
                    var msg = string.Format(
                        "Can not cast the source data type {0} to member type {1} of member {2}.",
                        value.GetType(), setup.MemberType, setup.MemberName);
                    throw new InvalidCastException(msg, e);
                }
            }

            return (T)obj; // if T is value type, unbox it here
        }

        private MemberSetupInfo BuildSetupInfo(int dataColIndex, MemberInfo memberInfo)
        {
            var info = new MemberSetupInfo { Index = dataColIndex };

            var propInfo = memberInfo as PropertyInfo;
            if (propInfo != null)
            {
                info.MemberType = propInfo.PropertyType;
                info.Setter = PropertyAccessorGenerator.CreateSetter(propInfo);
                info.MemberName = propInfo.Name;
            }
            else
            {
                var fieldInfo = (FieldInfo)memberInfo;
                info.MemberType = fieldInfo.FieldType;
                info.Setter = FieldAccessorGenerator.CreateSetter(fieldInfo);
                info.MemberName = fieldInfo.Name;
            }

            return info;
        }

        private struct MemberSetupInfo
        {
            public int Index;
            public string MemberName;
            public Type MemberType;
            public Action<object, object> Setter;
        }
    }
}
