using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using cmstar.RapidReflection.Emit;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// A mapper that maps data from a <see cref="IDataRecord"/> to
    /// the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type which the data is mapped to.</typeparam>
    public class ObjectMapper<T> : IMapper<T>
    {
        private readonly Func<object> _targetConstructor;

        private int _templateFieldCount;
        private IList<MemberSetupInfo> _targetMemberSetups;

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
            _targetMemberSetups = ParseMembers(type, template);
            _templateFieldCount = template.FieldCount;
        }

        public T MapRow(IDataRecord record, int rowNum)
        {
            ValidateTemplate(record);

            // don't cast obj to T here, if T is value type, a box is needed
            // since the value type will be clone into a box wrapper during boxing
            var obj = _targetConstructor();

            foreach (var setup in _targetMemberSetups)
            {
                var value = record.GetValue(setup.Index);
                if (value == null)
                    continue;

                // deal with DBNull, consider that DBNull should be mapped to CLR null,
                // but *NOT* for value types.
                // Users should use Nullable<> (e.g. int?) types for DBNulls.
                if (value == DBNull.Value)
                {
                    if (setup.CanBeNull)
                    {
                        continue; // the member is not set so it will remain null
                    }

                    // an error for un-nullable types
                    var msg = string.Format(
                        "Can not cast DBNull to member type {0} of member '{1}'.",
                        setup.MemberUnderlyingType, setup.MemberName);
                    throw new InvalidCastException(msg);
                }

                try
                {
                    var memberType = setup.MemberUnderlyingType;
                    var finalValue = value;

                    if (setup.IsEnum)
                    {
                        var stringValue = value as string;
                        if (stringValue == null)
                        {
                            finalValue = Enum.ToObject(memberType, value);
                        }
                        else
                        {
                            finalValue = Enum.Parse(memberType, stringValue);
                        }
                    }
                    else if (setup.NeedConvertType)
                    {
                        finalValue = Convert.ChangeType(value, memberType);
                    }

                    setup.Setter(obj, finalValue);
                }
                catch (Exception e)
                {
                    var valueType = value.GetType();
                    var msg = string.Format(
                        "Can not cast the source data type {0} to member type {1} of member '{2}'.",
                        valueType, setup.MemberUnderlyingType, setup.MemberName);
                    throw new InvalidCastException(msg, e);
                }
            }

            return (T)obj; // if T is value type, it is unboxed here
        }

        // Check the data row, if the row differs from the original template,
        // reparse MemberSetupInfo from the new row.
        // If a columns was added or removed from a table, some SQL, such as
        // 'SELECT * FROM some_table' will output difference result, which may
        // break up the MapRow method.
        private void ValidateTemplate(IDataRecord template)
        {
            // Currently we only check the field count.
            // Checking the field type for each column may be too slow.
            if (template.FieldCount == _templateFieldCount)
                return;

            var type = typeof(T);
            _targetMemberSetups = ParseMembers(type, template);
            _templateFieldCount = template.FieldCount;
        }

        private static IList<MemberSetupInfo> ParseMembers(Type type, IDataRecord template)
        {
            var props = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite);

#if NET35
            var propMap = MakeMemberMap(props.Cast<MemberInfo>());
#else
            var propMap = MakeMemberMap(props);
#endif

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var fieldMap = MakeMemberMap(fields);
            var targetMemberSetups = new List<MemberSetupInfo>();
            var flags = new bool[template.FieldCount]; // all false

            // 3-level match:
            // use a strict match
            Func<string, string, bool> fieldNameMemberNameMatches = (f, m) => f == m;
            AppendMemberSetups(targetMemberSetups, template, flags, propMap, fieldMap, fieldNameMemberNameMatches);

            // use a case-insensitive match
            fieldNameMemberNameMatches = (f, m) => StringComparer.OrdinalIgnoreCase.Compare(f, m) == 0;
            AppendMemberSetups(targetMemberSetups, template, flags, propMap, fieldMap, fieldNameMemberNameMatches);

            // compare by the underlying name of the field
            fieldNameMemberNameMatches = (f, m) => StringComparer.OrdinalIgnoreCase.Compare(GetUnderlyingName(f), m) == 0;
            AppendMemberSetups(targetMemberSetups, template, flags, propMap, fieldMap, fieldNameMemberNameMatches);

            return targetMemberSetups;
        }

        // groups memberInfos by the lowercase name
        private static Dictionary<string, List<MemberInfo>> MakeMemberMap(IEnumerable<MemberInfo> memberInfos)
        {
            var map = new Dictionary<string, List<MemberInfo>>();

            foreach (var memberInfo in memberInfos)
            {
                var underlyingName = GetUnderlyingName(memberInfo.Name);

                List<MemberInfo> memberList;
                if (map.TryGetValue(underlyingName, out memberList))
                {
                    memberList.Add(memberInfo);
                }
                else
                {
                    memberList = new List<MemberInfo> { memberInfo };
                    map.Add(underlyingName, memberList);
                }
            }

            return map;
        }

        // Scan the template record with an array of flags in which true means the field at the
        // corresponding index has already been used by a MemberSetupInfo and should be ignored.
        // Then, find a member with a matched name and setup.
        private static void AppendMemberSetups(
            List<MemberSetupInfo> memberSetups, IDataRecord template, bool[] flags,
            Dictionary<string, List<MemberInfo>> propMap, Dictionary<string, List<MemberInfo>> fieldMap,
            Func<string, string, bool> fieldNameMemberNameMatches)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i])
                    continue;

                var name = template.GetName(i);
                var underlyingName = GetUnderlyingName(name);

                List<MemberInfo> members;
                if (!propMap.TryGetValue(underlyingName, out members)
                    && !fieldMap.TryGetValue(underlyingName, out members))
                {
                    continue;
                }

                for (int j = 0; j < members.Count; j++)
                {
                    var member = members[j];
                    if (!fieldNameMemberNameMatches(name, member.Name))
                        continue;

                    var dataFieldType = template.GetFieldType(i);
                    memberSetups.Add(BuildSetupInfo(i, dataFieldType, member));
                    members.RemoveAt(j);
                    flags[i] = true;
                    break;
                }
            }
        }

        private static MemberSetupInfo BuildSetupInfo(int dataColIndex, Type dataFieldType, MemberInfo memberInfo)
        {
            var info = new MemberSetupInfo { Index = dataColIndex };

            var propInfo = memberInfo as PropertyInfo;
            if (propInfo != null)
            {
                var propertyType = propInfo.PropertyType;
                var underlyingType = ReflectionUtils.GetUnderlyingType(propertyType);
                info.MemberUnderlyingType = underlyingType;
                info.NeedConvertType = !propertyType.IsAssignableFrom(dataFieldType);
                info.IsEnum = underlyingType.IsEnum;
                info.CanBeNull = ReflectionUtils.IsNullable(propertyType);
                info.Setter = PropertyAccessorGenerator.CreateSetter(propInfo);
                info.MemberName = propInfo.Name;
            }
            else
            {
                var fieldInfo = (FieldInfo)memberInfo;
                var fieldType = fieldInfo.FieldType;
                var underlyingType = ReflectionUtils.GetUnderlyingType(fieldType);
                info.MemberUnderlyingType = underlyingType;
                info.NeedConvertType = !fieldType.IsAssignableFrom(dataFieldType);
                info.IsEnum = underlyingType.IsEnum;
                info.CanBeNull = ReflectionUtils.IsNullable(fieldType);
                info.Setter = FieldAccessorGenerator.CreateSetter(fieldInfo);
                info.MemberName = fieldInfo.Name;
            }

            return info;
        }

        // gets a name that ignores the internal underline character and then to lower case
        // e.g. _Abc_deF => _abcdef
        private static string GetUnderlyingName(string name)
        {
            var sb = new StringBuilder(name.Length);

            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];

                if (i != 0 && i != name.Length && c == '_')
                    continue;

                c = char.ToLower(c);
                sb.Append(c);
            }

            return sb.ToString();
        }

        private struct MemberSetupInfo
        {
            public int Index;
            public string MemberName;
            public Type MemberUnderlyingType;
            public bool NeedConvertType;
            public bool IsEnum;
            public bool CanBeNull;
            public Action<object, object> Setter;
        }
    }
}
