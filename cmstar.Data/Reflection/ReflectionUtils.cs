using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace cmstar.Data.Reflection
{
    /// <summary>
    /// 包含类型反射判断相关的方法。
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// 判断给定类型是否可被赋值为null。
        /// </summary>
        /// <param name="t">类型。</param>
        /// <returns>若类型为引用类型，返回<c>true</c>；否则返回<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="t"/>为<c>null</c>。</exception>
        public static bool IsNullable(Type t)
        {
            ArgAssert.NotNull(t, "t");
            return !t.IsValueType || IsNullableType(t);
        }

        /// <summary>
        /// 判断给定类型是否是<see cref="System.Nullable{T}"/>的实例。
        /// </summary>
        /// <param name="t">类型。</param>
        /// <returns>若类型为<see cref="System.Nullable{T}"/>，返回<c>true</c>；否则返回<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="t"/>为<c>null</c>。</exception>
        public static bool IsNullableType(Type t)
        {
            ArgAssert.NotNull(t, "t");
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// 获取类型或类型内包含的不可空类型（当类型为<see cref="System.Nullable{T}"/>时）。
        /// </summary>
        /// <param name="t">类型。</param>
        /// <returns>最终类型，若类型为<see cref="System.Nullable{T}"/>，返回其内部类型。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="t"/>为<c>null</c>。</exception>
        public static Type GetUnderlyingType(Type t)
        {
            return IsNullableType(t) ? Nullable.GetUnderlyingType(t) : t;
        }

        /// <summary>
        /// 判断给定类型是否是目标类型或是目标类型的子类。
        /// </summary>
        /// <param name="thisType">待判断类型。</param>
        /// <param name="targetType">目标类型。</param>
        /// <returns>若给定类型是目标类型或是目标类型的子类，返回<c>true</c>；否则返回<c>false</c>。</returns>
        /// <exception cref="ArgumentNullException">当<paramref name="thisType"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentNullException">当<paramref name="targetType"/>为<c>null</c>。</exception>
        public static bool IsOrIsSubClassOf(Type thisType, Type targetType)
        {
            ArgAssert.NotNull(thisType, "thisType");
            ArgAssert.NotNull(targetType, "targetType");

            return thisType == targetType || thisType.IsSubclassOf(targetType);
        }

        /// <summary>
        /// 从给定的类型上解析泛型参数表，并指定解析参数时所使用的泛型类型定义。
        /// </summary>
        /// <param name="type">从此类型上解析泛型参数表。</param>
        /// <param name="genericTypeDefinition">泛型类型定义。可以是接口，也可以是非接口类型。</param>
        /// <returns>类型的泛型参数的数组，其元素顺序与类型定义的顺序一致。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="genericTypeDefinition"/>为<c>null</c>。</exception>
        /// <exception cref="ArgumentException"><paramref name="genericTypeDefinition"/>不是泛型类型定义。</exception>
        /// <example>
        /// 一个类型可以继承多个泛型类型，例如：
        /// GenericClass{T} : IDictionary{T, int}, ICollection{string}
        /// 此时，对于GenericClass{long}，使用泛型定义IDictionary{,}，获取参数表类型是 [long, int]；
        /// 而对于泛型定义ICollection{}则结果是 [string]；
        /// </example>
        public static Type[] GetGenericArguments(Type type, Type genericTypeDefinition)
        {
            ArgAssert.NotNull(type, "type");
            ArgAssert.NotNull(genericTypeDefinition, "genericTypeDefinition");

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                var msg = string.Format(
                    "The type {0} is not a generic type definition.",
                    genericTypeDefinition.Name);
                throw new ArgumentException(msg, "genericTypeDefinition");
            }

            if (genericTypeDefinition.IsInterface)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
                    return type.GetGenericArguments();

                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                        continue;

                    if (interfaceType.GetGenericTypeDefinition() != genericTypeDefinition)
                        continue;

                    return interfaceType.GetGenericArguments();
                }
            }
            else
            {
                var baseType = type;
                do
                {
                    if (!baseType.IsGenericType)
                        continue;

                    if (baseType.GetGenericTypeDefinition() != genericTypeDefinition)
                        continue;

                    return baseType.GetGenericArguments();

                } while ((baseType = baseType.BaseType) != null);
            }

            return null;
        }

        /// <summary>
        /// 判断给定的类型是否是匿名类型。
        /// </summary>
        /// <param name="type">待判断的类型。</param>
        /// <returns>true若给定的类型是匿名类型；否则为false。</returns>
        public static bool IsAnonymousType(Type type)
        {
            ArgAssert.NotNull(type, "type");

            if (!type.IsGenericType)
                return false;

            if (!Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false))
                return false;

            if ((type.Attributes & TypeAttributes.NotPublic) != TypeAttributes.NotPublic)
                return false;

            return type.Name.Contains("AnonymousType");
        }
    }
}
