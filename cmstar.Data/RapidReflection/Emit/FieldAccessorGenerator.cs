using System;
using System.Reflection;

namespace cmstar.Data.RapidReflection.Emit
{
    public static class FieldAccessorGenerator
    {
        /// <summary>
        /// Creates a dynamic method for getting the value of the given field.
        /// </summary>
        /// <param name="fieldInfo">
        /// The instance of <see cref="FieldInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given field.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="fieldInfo"/> is null.</exception>
        public static Func<object, object> CreateGetter(FieldInfo fieldInfo)
        {
            return CreateGetter<object, object>(fieldInfo);
        }

        /// <summary>
        /// Creates a dynamic method for getting the value of the given field.
        /// </summary>
        /// <typeparam name="TSource">The type of the intance from which to get the value.</typeparam>
        /// <typeparam name="TRet">The type of the return value.</typeparam>
        /// <param name="fieldInfo">
        /// The instance of <see cref="FieldInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given field.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="fieldInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <typeparamref name="TSource"/> is not <see cref="object"/>, and from which 
        /// the declaring type of the field is not assignable.
        /// -or-
        /// <typeparamref name="TRet"/> is not assignable from the type of the field.
        /// </exception>
        public static Func<TSource, TRet> CreateGetter<TSource, TRet>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var identity = new { fieldInfo, sourceType = typeof(TSource), returnType = typeof(TRet) };
            var getter = (Func<TSource, TRet>)DelegateCache.GetOrAdd(
                identity, x => DoCreateGetter<TSource, TRet>(fieldInfo));

            return getter;
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given field.
        /// </summary>
        /// <param name="fieldInfo">
        /// The instance of <see cref="FieldInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given field.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="fieldInfo"/> is null.</exception>
        /// <remarks>
        /// In order to set a field on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<object, object> CreateSetter(FieldInfo fieldInfo)
        {
            return CreateSetter<object, object>(fieldInfo);
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given field.
        /// </summary>
        /// <typeparam name="TTarget">The type of the instance the field belongs to.</typeparam>
        /// <typeparam name="TValue">The type of the field value to set.</typeparam>
        /// <param name="fieldInfo">
        /// The instance of <see cref="FieldInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given field.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="fieldInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <typeparamref name="TTarget"/> is a value type.
        /// -or- 
        /// <typeparamref name="TTarget"/> is not <see cref="object"/>, and from which 
        /// the declaring type of <paramref name="fieldInfo"/> is not assignable.
        /// -or-
        /// <typeparamref name="TValue"/> is not <see cref="object"/>, and the type of field 
        /// is not assignable from <typeparamref name="TValue"/>.
        /// </exception>
        /// <remarks>
        /// In order to set a field on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<TTarget, TValue> CreateSetter<TTarget, TValue>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            var identity = new { fieldInfo, targetType = typeof(TTarget), valueType = typeof(TValue) };
            var setter = (Action<TTarget, TValue>)DelegateCache.GetOrAdd(
                identity, x => DoCreateSetter<TTarget, TValue>(fieldInfo));

            return setter;
        }

        private static Func<TSource, TRet> DoCreateGetter<TSource, TRet>(FieldInfo fieldInfo)
        {
            if (typeof(TSource) != typeof(object)
                && !fieldInfo.DeclaringType.IsAssignableFrom(typeof(TSource)))
            {
                throw new ArgumentException(
                    "The field's declaring type is not assignable from the type of the instance.",
                    "fieldInfo");
            }

            if (!typeof(TRet).IsAssignableFrom(fieldInfo.FieldType))
            {
                throw new ArgumentException(
                    "The type of the return value is not assignable from the type of the field.",
                    "fieldInfo");
            }

            return EmitFieldGetter<TSource, TRet>(fieldInfo);
        }

        private static Action<TTarget, TValue> DoCreateSetter<TTarget, TValue>(FieldInfo fieldInfo)
        {
            if (typeof(TTarget).IsValueType)
            {
                throw new ArgumentException(
                    "The type of the isntance should not be a value type. " +
                    "For a value type, use System.Object instead.",
                    "fieldInfo");
            }

            if (typeof(TTarget) != typeof(object)
                && !fieldInfo.DeclaringType.IsAssignableFrom(typeof(TTarget)))
            {
                throw new ArgumentException(
                    "The declaring type of the field is not assignable from the type of the isntance.",
                    "fieldInfo");
            }

            if (typeof(TValue) != typeof(object)
                && !fieldInfo.FieldType.IsAssignableFrom(typeof(TValue)))
            {
                throw new ArgumentException(
                    "The type of the field is not assignable from the type of the value.",
                    "fieldInfo");
            }

            return EmitFieldSetter<TTarget, TValue>(fieldInfo);
        }

        private static Func<TSource, TReturn> EmitFieldGetter<TSource, TReturn>(FieldInfo fieldInfo)
        {
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Get" + fieldInfo.Name,
                typeof(TReturn),
                new[] { typeof(TSource) },
                fieldInfo.DeclaringType);
            var il = dynamicMethod.GetILGenerator();

            if (fieldInfo.IsStatic)
            {
                il.Ldsfld(fieldInfo);
            }
            else
            {
                //unbox the source if needed
                if (typeof(TSource).IsValueType)
                {
                    il.Ldarga_S(0);
                }
                else
                {
                    il.Ldarg_0();
                    il.CastReference(fieldInfo.DeclaringType);
                }

                il.Ldfld(fieldInfo);
            }

            //box the return value if needed
            if (!typeof(TReturn).IsValueType && fieldInfo.FieldType.IsValueType)
            {
                il.Box(fieldInfo.FieldType);
            }
            il.Ret();

            return (Func<TSource, TReturn>)dynamicMethod.CreateDelegate(typeof(Func<TSource, TReturn>));
        }

        private static Action<TTarget, TValue> EmitFieldSetter<TTarget, TValue>(FieldInfo fieldInfo)
        {
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Set" + fieldInfo.Name,
                null,
                new[] { typeof(TTarget), typeof(TValue) },
                fieldInfo.DeclaringType);
            var il = dynamicMethod.GetILGenerator();

            //copy the value to a local variable, unbox if needed
            il.DeclareLocal(fieldInfo.FieldType);
            il.Ldarg_1();
            if (!typeof(TValue).IsValueType)
            {
                il.CastValue(fieldInfo.FieldType);
            }
            il.Stloc_0();

            if (fieldInfo.IsStatic)
            {
                il.Ldloc_0();
                il.Stsfld(fieldInfo);
            }
            else
            {
                il.Ldarg_0();
                il.CastReference(fieldInfo.DeclaringType);
                il.Ldloc_0();
                il.Stfld(fieldInfo);
            }

            il.Ret();
            return (Action<TTarget, TValue>)dynamicMethod.CreateDelegate(typeof(Action<TTarget, TValue>));
        }
    }
}
