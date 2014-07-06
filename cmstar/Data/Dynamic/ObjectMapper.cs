using System;
using System.Collections;
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

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite).Cast<MemberInfo>();
            var propMap = MakeMemberMap(props);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var fieldMap = MakeMemberMap(fields);

            _targetMemberSetups = new List<MemberSetupInfo>();
            var flags = new bool[template.FieldCount];

            // 3-level match
            // first use a strict match
            Func<string, string, bool> fieldNameMemberNameMatches = (f, m) => f == m;
            AppendMemberSetups(template, flags, propMap, fieldMap, fieldNameMemberNameMatches);

            // use a case-insensitive match
            fieldNameMemberNameMatches = (f, m) => StringComparer.OrdinalIgnoreCase.Compare(f, m) == 0;
            AppendMemberSetups(template, flags, propMap, fieldMap, fieldNameMemberNameMatches);

            // ignores the underline in the field and then use a case-insensitive match
            fieldNameMemberNameMatches = (f, m) =>
                StringComparer.OrdinalIgnoreCase.Compare(f.Replace("_", ""), m) == 0;
            AppendMemberSetups(template, flags, propMap, fieldMap, fieldNameMemberNameMatches);
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

        // groups memberInfos by memberInfo.Name.Replace("_", "").ToLower()
        private Dictionary<string, List<MemberInfo>> MakeMemberMap(IEnumerable<MemberInfo> memberInfos)
        {
            var map = new Dictionary<string, List<MemberInfo>>();

            foreach (var memberInfo in memberInfos)
            {
                var name = memberInfo.Name.Replace("_", "").ToLower();

                List<MemberInfo> memberList;
                if (map.TryGetValue(name, out memberList))
                {
                    memberList.Add(memberInfo);
                }
                else
                {
                    memberList = new List<MemberInfo> { memberInfo };
                    map.Add(name, memberList);
                }
            }

            return map;
        }

        // Scan the template record with an array of flags in which true means the field at the
        // corresbonding index has already been used by a MemberSetupInfo and should be ingored.
        // Then, find a member with a matched name and setup.
        private void AppendMemberSetups(IDataRecord template, bool[] flags,
            Dictionary<string, List<MemberInfo>> propMap, Dictionary<string, List<MemberInfo>> fieldMap,
            Func<string, string, bool> fieldNameMemberNameMatches)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i])
                    continue;

                var name = template.GetName(i);
                var pattern = name.Replace("_", "").ToLower();

                List<MemberInfo> members;
                if (!propMap.TryGetValue(pattern, out members) && !fieldMap.TryGetValue(pattern, out members))
                    continue;
                
                for (int j = 0; j < members.Count; j++)
                {
                    var member = members[j];
                    if (!fieldNameMemberNameMatches(name, member.Name))
                        continue;

                    _targetMemberSetups.Add(BuildSetupInfo(i, member));
                    members.RemoveAt(j);
                    flags[i] = true;
                    break;
                }
            }
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
