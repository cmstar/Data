using System;
using System.Reflection;

namespace cmstar.Data.Reflection.Emit
{
    internal static class PropertyAccessorGenerator
    {
        /// <summary>
        /// Creates a dynamic method for getting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer.
        /// -or-
        /// The get accessor method from <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        public static Func<object, object> CreateGetter(PropertyInfo propertyInfo)
        {
            return CreateGetter(propertyInfo, true);
        }

        /// <summary>
        /// Creates a dynamic method for getting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <param name="nonPublic">
        /// Indicates whether to use the non-public property getter method.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer.
        /// -or-
        /// The get accessor method from <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        public static Func<object, object> CreateGetter(PropertyInfo propertyInfo, bool nonPublic)
        {
            return CreateGetter<object, object>(propertyInfo, nonPublic);
        }

        /// <summary>
        /// Creates a dynamic method for getting the value of the given property.
        /// </summary>
        /// <typeparam name="TSource">The type of the intance from which to get the value.</typeparam>
        /// <typeparam name="TRet">The type of the return value.</typeparam>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <param name="nonPublic">
        /// Indicates whether to use the non-public property getter method.
        /// </param>
        /// <returns>
        /// A dynamic method for getting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer.
        /// -or -
        /// The get accessor method from <paramref name="propertyInfo"/> cannot be retrieved.
        /// -or-
        /// <typeparamref name="TSource"/> is not <see cref="object"/>, and from which 
        /// the declaring type the property is not assignable.
        /// -or-
        /// <typeparamref name="TRet"/> is not assignable from the property type.
        /// </exception>
        public static Func<TSource, TRet> CreateGetter<TSource, TRet>(PropertyInfo propertyInfo, bool nonPublic)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var identity = new { propertyInfo, nonPublic, sourceType = typeof(TSource), returnType = typeof(TRet) };
            var getter = (Func<TSource, TRet>)DelegateCache.GetOrAdd(
                identity, x => DoCreateGetter<TSource, TRet>(propertyInfo, nonPublic));

            return getter;
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer.
        /// -or-
        /// The set accessor method from the <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        /// <remarks>
        /// In order to set a property value on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<object, object> CreateSetter(PropertyInfo propertyInfo)
        {
            return CreateSetter(propertyInfo, true);
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given property.
        /// </summary>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <param name="nonPublic">
        /// Indicates whether to use the non-public property setter method.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer.
        /// -or-
        /// The set accessor method from the <paramref name="propertyInfo"/> cannot be retrieved.
        /// </exception>
        /// <remarks>
        /// In order to set a property value on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<object, object> CreateSetter(PropertyInfo propertyInfo, bool nonPublic)
        {
            return CreateSetter<object, object>(propertyInfo, nonPublic);
        }

        /// <summary>
        /// Creates a dynamic method for setting the value of the given property.
        /// </summary>
        /// <typeparam name="TTarget">The type of the instance the property belongs to.</typeparam>
        /// <typeparam name="TValue">The type of the value to set.</typeparam>
        /// <param name="propertyInfo">
        /// The instance of <see cref="PropertyInfo"/> from which the dynamic method would be created.
        /// </param>
        /// <param name="nonPublic">
        /// Indicates whether to use the non-public property setter method.
        /// </param>
        /// <returns>
        /// A dynamic method for setting the value of the given property.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The property is an indexer.
        /// -or-
        /// The set accessor method from the <paramref name="propertyInfo"/> cannot be retrieved.
        /// -or-
        /// <typeparamref name="TTarget"/> is a value type.
        /// -or-
        /// <typeparamref name="TTarget"/> is not <see cref="object"/>, and from which 
        /// the declaring type of <paramref name="propertyInfo"/> is not assignable.
        /// -or-
        /// <typeparamref name="TValue"/> is not <see cref="object"/>, and the type of property 
        /// is not assignable from <typeparamref name="TValue"/>. 
        /// </exception>
        /// <remarks>
        /// In order to set a property value on a value type succesfully, the value type must be boxed 
        /// in and <see cref="object"/>, and unboxed from the object after the dynamic
        /// set mothod is called, e.g.
        /// <code>
        ///   object boxedStruct = new SomeStruct();
        ///   setter(s, "the value");
        ///   SomeStruct unboxedStruct = (SomeStruct)boxedStruct;
        /// </code>
        /// </remarks>
        public static Action<TTarget, TValue> CreateSetter<TTarget, TValue>(PropertyInfo propertyInfo, bool nonPublic)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var identity = new { propertyInfo, nonPublic, targetType = typeof(TTarget), valueType = typeof(TValue) };
            var setter = (Action<TTarget, TValue>)DelegateCache.GetOrAdd(
                identity, x => DoCreateSetter<TTarget, TValue>(propertyInfo, nonPublic));

            return setter;
        }

        private static Func<TSource, TRet> DoCreateGetter<TSource, TRet>(PropertyInfo propertyInfo, bool nonPublic)
        {
            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                throw new ArgumentException(
                    "Cannot create a dynamic getter for an indexed property.",
                    "propertyInfo");
            }

            if (typeof(TSource) != typeof(object)
                && !propertyInfo.DeclaringType.IsAssignableFrom(typeof(TSource)))
            {
                throw new ArgumentException(
                    "The declaring type of the property is not assignable from the type of the instance.",
                    "propertyInfo");
            }

            if (!typeof(TRet).IsAssignableFrom(propertyInfo.PropertyType))
            {
                throw new ArgumentException(
                    "The type of the return value is not assignable from the type of the property.",
                    "propertyInfo");
            }

            //the method call of the get accessor method fails in runtime 
            //if the declaring type of the property is an interface and TSource is a value type, 
            //in this case, we should find the property from TSource whose DeclaringType is TSource itself
            if (typeof(TSource).IsValueType && propertyInfo.DeclaringType.IsInterface)
            {
                propertyInfo = typeof(TSource).GetProperty(propertyInfo.Name);
            }

            var getMethod = propertyInfo.GetGetMethod(nonPublic);
            if (getMethod == null)
            {
                if (nonPublic)
                {
                    throw new ArgumentException(
                        "The property does not have a get method.", "propertyInfo");
                }

                throw new ArgumentException(
                    "The property does not have a public get method.", "propertyInfo");
            }

            return EmitPropertyGetter<TSource, TRet>(propertyInfo, getMethod);
        }

        private static Action<TTarget, TValue> DoCreateSetter<TTarget, TValue>(PropertyInfo propertyInfo, bool nonPublic)
        {
            if (typeof(TTarget).IsValueType)
            {
                throw new ArgumentException(
                    "The type of the isntance should not be a value type. " +
                    "For a value type, use System.Object instead.",
                    "propertyInfo");
            }

            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                throw new ArgumentException(
                    "Cannot create a dynamic setter for an indexed property.",
                    "propertyInfo");
            }

            if (typeof(TTarget) != typeof(object)
                && !propertyInfo.DeclaringType.IsAssignableFrom(typeof(TTarget)))
            {
                throw new ArgumentException(
                    "The declaring type of the property is not assignable from the type of the isntance.",
                    "propertyInfo");
            }

            if (typeof(TValue) != typeof(object)
                && !propertyInfo.PropertyType.IsAssignableFrom(typeof(TValue)))
            {
                throw new ArgumentException(
                    "The type of the property is not assignable from the type of the value.",
                    "propertyInfo");
            }

            var setMethod = propertyInfo.GetSetMethod(nonPublic);
            if (setMethod == null)
            {
                if (nonPublic)
                {
                    throw new ArgumentException(
                        "The property does not have a set method.", "propertyInfo");
                }

                throw new ArgumentException(
                    "The property does not have a public set method.", "propertyInfo");
            }

            return EmitPropertySetter<TTarget, TValue>(propertyInfo, setMethod);
        }

        private static Func<TSource, TReturn> EmitPropertyGetter<TSource, TReturn>(
            PropertyInfo propertyInfo, MethodInfo getMethod)
        {
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Get" + propertyInfo.Name,
                typeof(TReturn),
                new[] { typeof(TSource) },
                propertyInfo.DeclaringType);
            var il = dynamicMethod.GetILGenerator();

            if (!getMethod.IsStatic)
            {
                //unbox the input value if needed
                if (typeof(TSource).IsValueType)
                {
                    il.Ldarga_S(0);
                }
                else
                {
                    il.Ldarg_0();
                    il.CastReference(propertyInfo.DeclaringType);
                }
            }

            il.CallMethod(getMethod);

            //box the return value if needed
            if (!typeof(TReturn).IsValueType && propertyInfo.PropertyType.IsValueType)
            {
                il.Box(propertyInfo.PropertyType);
            }

            il.Ret();

            return (Func<TSource, TReturn>)dynamicMethod.CreateDelegate(typeof(Func<TSource, TReturn>));
        }

        private static Action<TTarget, TValue> EmitPropertySetter<TTarget, TValue>(
            PropertyInfo propertyInfo, MethodInfo setMethod)
        {
            var propType = propertyInfo.PropertyType;
            var declaringType = propertyInfo.DeclaringType;
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Set" + propertyInfo.Name,
                null,
                new[] { typeof(TTarget), typeof(TValue) },
                declaringType);
            var il = dynamicMethod.GetILGenerator();

            //copy the value to a local variable, unbox if needed
            il.DeclareLocal(propType);
            il.Ldarg_1();
            if (!typeof(TValue).IsValueType)
            {
                il.CastValue(propType);
            }
            il.Stloc_0();

            //push the instance, unbox if needed
            if (!setMethod.IsStatic)
            {
                il.Ldarg_0();
                il.CastReference(declaringType);
            }

            //push the value and call the method
            il.Ldloc_0();
            il.CallMethod(setMethod);
            il.Ret();

            return (Action<TTarget, TValue>)dynamicMethod.CreateDelegate(typeof(Action<TTarget, TValue>));
        }
    }
}
