using System;
using System.Reflection;
using System.Reflection.Emit;

namespace cmstar.Data.Reflection.Emit
{
    internal static class EmitUtils
    {
        /// <summary>
        /// Creates an instance of <see cref="DynamicMethod"/>.
        /// </summary>
        /// <param name="methodName">The name of the dynamic method.</param>
        /// <param name="returnType">
        /// The return type of the dynamic method, null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type"/> specifying the types of the parameters of the dynamic method, 
        /// or null if the method has no parameters. 
        /// </param>
        /// <param name="owner">
        /// Specifies with which type the dynamic method is to be logically associated,
        /// if the type is an interface, the dynamic method will be associated to the module.
        /// </param>
        public static DynamicMethod CreateDynamicMethod(
            string methodName, Type returnType, Type[] parameterTypes, Type owner)
        {
            var dynamicMethod = owner.IsInterface ?
                new DynamicMethod(methodName, returnType, parameterTypes, owner.Module, true) :
                new DynamicMethod(methodName, returnType, parameterTypes, owner, true);

            return dynamicMethod;
        }

        /// <summary>
        /// Performs a boxing operation if the given type is a value type.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="type">The type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator BoxIfNeeded(this ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Box(type);
            }

            return il;
        }

        /// <summary>
        /// Performs a unboxing operation (unbox.any) if the given type is a value type.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="type">The type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator UnBoxIfNeeded(this ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Unbox_Any(type);
            }

            return il;
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="methodInfo">The method descriptor.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator CallMethod(this ILGenerator il, MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            if (methodInfo.IsVirtual)
            {
                il.Callvirt(methodInfo);
            }
            else
            {
                il.Call(methodInfo);
            }

            return il;
        }

        /// <summary>
        /// Casts an object or value type passed by reference to the specified type
        /// and pushes the result onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="targetType">The type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator CastValue(this ILGenerator il, Type targetType)
        {
            il.Unbox_Any(targetType);
            return il;
        }

        /// <summary>
        /// Casts an object or value type passed by reference to the specified type
        /// and pushes the object reference or the value type pointer of the result
        /// onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="targetType">The type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator CastReference(this ILGenerator il, Type targetType)
        {
            if (targetType.IsValueType)
            {
                il.Unbox(targetType);
            }
            else
            {
                il.Castclass(targetType);
            }

            return il;
        }

        /// <summary>
        /// Loads the argument at the specified index onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator LoadArgument(this ILGenerator il, short index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "The index should not be less than zero.");

            switch (index)
            {
                case 0: il.Ldarg_0(); break;
                case 1: il.Ldarg_1(); break;
                case 2: il.Ldarg_2(); break;
                case 3: il.Ldarg_3(); break;

                default:
                    if (index <= byte.MaxValue)
                    {
                        il.Ldarg_S((byte)index);
                    }
                    else
                    {
                        il.Ldarg(index);
                    }
                    break;
            }

            return il;
        }

        /// <summary>
        /// Loads the address of the argument at the specified index onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index"></param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator LoadArgumentAddress(this ILGenerator il, short index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "The index should not be less than zero.");

            if (index <= byte.MaxValue)
            {
                il.Ldarga_S((byte)index);
            }
            else
            {
                il.Ldarga(index);
            }

            return il;
        }

        /// <summary>
        /// Pushes the integer value onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="i">The integer value.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator LoadInt32(this ILGenerator il, int i)
        {
            switch (i)
            {
                case -1: il.Ldc_I4_M1(); break;
                case 0: il.Ldc_I4_0(); break;
                case 1: il.Ldc_I4_1(); break;
                case 2: il.Ldc_I4_2(); break;
                case 3: il.Ldc_I4_3(); break;
                case 4: il.Ldc_I4_4(); break;
                case 5: il.Ldc_I4_5(); break;
                case 6: il.Ldc_I4_6(); break;
                case 7: il.Ldc_I4_7(); break;
                case 8: il.Ldc_I4_8(); break;

                default:
                    if (i <= byte.MaxValue)
                    {
                        il.Ldc_I4_S((byte)i);
                    }
                    else
                    {
                        il.Ldc_I4(i);
                    }
                    break;
            }

            return il;
        }

        /// <summary>
        /// Loads the local variable at the specified index onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index of the local variable.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator LoadLocalVariable(this ILGenerator il, short index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "The index should not be less than zero.");

            switch (index)
            {
                case 0: il.Ldloc_0(); break;
                case 1: il.Ldloc_1(); break;
                case 2: il.Ldloc_2(); break;
                case 3: il.Ldloc_3(); break;

                default:
                    if (index <= byte.MaxValue)
                    {
                        il.Ldloc_S((byte)index);
                    }
                    else
                    {
                        il.Ldloc(index);
                    }
                    break;
            }

            return il;
        }

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index of the local variable.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator LoadLocalVariableAddress(this ILGenerator il, short index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "The index should not be less than zero.");

            if (index <= byte.MaxValue)
            {
                il.Ldloca_S((byte)index);
            }
            else
            {
                il.Ldloca(index);
            }

            return il;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="paramName">The parameter name used to initialize the exception instance.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator ThrowArgumentsNullExcpetion(this ILGenerator il, string paramName)
        {
            il.Ldstr(paramName);
            il.Newobj(typeof(ArgumentNullException).GetConstructor(new[] { typeof(string) }));
            il.Throw();

            return il;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/>, specifying the message and the parameter name.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="message">The message.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator ThrowArgumentsExcpetion(this ILGenerator il, string message, string paramName)
        {
            il.Ldstr(message);
            il.Ldstr(paramName);
            il.Newobj(typeof(ArgumentException).GetConstructor(new[] { typeof(string), typeof(string) }));
            il.Throw();

            return il;
        }
    }
}
