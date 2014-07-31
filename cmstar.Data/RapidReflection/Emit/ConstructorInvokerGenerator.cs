using System;
using System.Reflection;

namespace cmstar.Data.RapidReflection.Emit
{
    public static class ConstructorInvokerGenerator
    {
        /// <summary>
        /// Creates a dynamic method for creating instances of the given type.
        /// The given type must have a public parameterless constructor.
        /// </summary>
        /// <param name="type">The type of the instances to be created.</param>
        /// <returns>
        /// A dynamic method for creating instances of the given type.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The given type is an interface, or is abstract, 
        /// or does not have a public parameterless constructor.
        /// </exception>
        public static Func<object> CreateDelegate(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsInterface)
                throw new ArgumentException("The type is an interface.", "type");

            if (type.IsAbstract)
                throw new ArgumentException("The type is abstract.", "type");

            ConstructorInfo constructorInfo = null;
            if (type.IsClass)
            {
                constructorInfo = type.GetConstructor(Type.EmptyTypes);

                if (constructorInfo == null)
                    throw new ArgumentException(
                        "The type does not have a public parameterless constructor.", "type");
            }

            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Create" + type, typeof(object), Type.EmptyTypes, type);
            var il = dynamicMethod.GetILGenerator();

            if (type.IsClass)
            {
                il.Newobj(constructorInfo);
            }
            else //value type
            {
                il.DeclareLocal(type);
                il.LoadLocalVariableAddress(0);
                il.Initobj(type);

                il.LoadLocalVariable(0);
                il.Box(type);
            }

            il.Ret();
            return (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>));
        }

        /// <summary>
        /// Creates a dynamic method for creating instances from the given <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructorInfo">The constructor from which instances will be created.</param>
        /// <returns>
        /// A dynamic method for creating instances from the given constructor, the method receives an
        /// array as the arguments of the constructor.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constructorInfo"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The declaring type of the construcor is abstract.
        /// </exception>
        public static Func<object[], object> CreateDelegate(ConstructorInfo constructorInfo)
        {
            return CreateDelegate(constructorInfo, true);
        }

        /// <summary>
        /// Creates a dynamic method for creating instances from the given <see cref="ConstructorInfo"/>
        /// and indicates whether to perform a arguments validation in the dynamic method.
        /// </summary>
        /// <param name="constructorInfo">The constructor from which instances will be created.</param>
        /// <param name="validateArguments">
        /// If <c>true</c>, the dynamic method will validate if the array of arguments is null
        /// and check the length of the array to avoid the exceptions such as 
        /// <see cref="NullReferenceException"/> or <see cref="IndexOutOfRangeException"/>,
        /// an <see cref="ArgumentNullException"/> or <see cref="ArgumentException"/> will be thrown instead.
        /// </param>
        /// <returns>
        /// A dynamic method for creating instances from the given constructor, the method receives an
        /// array as the arguments of the constructor.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constructorInfo"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The declaring type of the construcor is abstract.
        /// </exception>
        public static Func<object[], object> CreateDelegate(ConstructorInfo constructorInfo, bool validateArguments)
        {
            if (constructorInfo == null)
                throw new ArgumentNullException("constructorInfo");

            var delclaringType = constructorInfo.DeclaringType;
            if (delclaringType.IsAbstract)
            {
                throw new ArgumentException(
                    "The declaring type of the constructor is abstract.", "constructorInfo");
            }

            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Create" + delclaringType.Name,
                typeof(object),
                new[] { typeof(object[]) },
                constructorInfo.DeclaringType);
            var il = dynamicMethod.GetILGenerator();

            var args = constructorInfo.GetParameters();
            var lableValidationCompleted = il.DefineLabel();
            if (!validateArguments || args.Length == 0)
            {
                il.Br_S(lableValidationCompleted);
            }
            else
            {
                var lableCheckArgumentsLength = il.DefineLabel();

                // if (arguments == null) throw new ArgumentNullExcpeiton("arguments");
                il.Ldarg_0();
                il.Brtrue_S(lableCheckArgumentsLength);

                il.ThrowArgumentsNullExcpetion("arguments");

                // if (arguments.Length < $(args.Length)) throw new ArgumentExcpeiton(msg, "arguments");
                il.MarkLabel(lableCheckArgumentsLength);
                il.Ldarg_0();
                il.Ldlen();
                il.Conv_I4();
                il.LoadInt32(args.Length);
                il.Bge_S(lableValidationCompleted);

                il.ThrowArgumentsExcpetion("Not enough arguments in the argument array.", "arguments");
            }

            il.MarkLabel(lableValidationCompleted);
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    il.Ldarg_0();
                    il.LoadInt32((short)i);
                    il.Ldelem_Ref();
                    il.CastValue(args[i].ParameterType);
                }
            }
            il.Newobj(constructorInfo);
            il.Ret();

            return (Func<object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object[], object>));
        }
    }
}
