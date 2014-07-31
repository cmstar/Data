using System;
using System.Reflection;

namespace cmstar.Data.RapidReflection.Emit
{
    public static class MethodInvokerGenerator
    {
        /// <summary>
        /// Creates a dynamic method for invoking the method from the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">
        /// The instance of <see cref="MemberInfo"/> from which the dyanmic method is to be created.
        /// </param>
        /// <returns>
        /// The delegate has two parameters: the first for the object instance (will be ignored 
        /// if the method is static), and the second for the arguments of the method (will be 
        /// ignored if the method has no arguments)/
        /// The return value of the delegate will be <c>null</c> if the method has no return value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="methodInfo"/> is null.</exception>
        public static Func<object, object[], object> CreateDelegate(MethodInfo methodInfo)
        {
            return CreateDelegate(methodInfo, true);
        }

        /// <summary>
        /// Creates a dynamic method for invoking the method from the given <see cref="MethodInfo"/>
        /// and indicates whether to perform a arguments validation in the dynamic method.
        /// </summary>
        /// <param name="methodInfo">
        /// The instance of <see cref="MemberInfo"/> from which the dyanmic method is to be created.
        /// </param>
        /// <param name="validateArguments">
        /// If <c>true</c>, the dynamic method will validate if the instance or the array of arguments 
        /// is null and check the length of the array to avoid the exceptions such as 
        /// <see cref="NullReferenceException"/> or <see cref="IndexOutOfRangeException"/>,
        /// an <see cref="ArgumentNullException"/> or <see cref="ArgumentException"/> will be thrown instead.
        /// </param>
        /// <returns>
        /// The delegate has two parameters: the first for the object instance (will be ignored 
        /// if the method is static), and the second for the arguments of the method (will be 
        /// ignored if the method has no arguments)/
        /// The return value of the delegate will be <c>null</c> if the method has no return value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="methodInfo"/> is null.</exception>
        public static Func<object, object[], object> CreateDelegate(
            MethodInfo methodInfo, bool validateArguments)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            var args = methodInfo.GetParameters();
            var dynamicMethod = EmitUtils.CreateDynamicMethod(
                "$Call" + methodInfo.Name,
                typeof(object),
                new[] { typeof(object), typeof(object[]) },
                methodInfo.DeclaringType);
            var il = dynamicMethod.GetILGenerator();

            var lableValidationCompleted = il.DefineLabel();
            if (!validateArguments || (methodInfo.IsStatic && args.Length == 0))
            {
                il.Br_S(lableValidationCompleted); //does not need validation
            }
            else
            {
                var lableCheckArgumentsRef = il.DefineLabel();
                var lableCheckArgumentsLength = il.DefineLabel();

                //check if the instance is null
                if (!methodInfo.IsStatic)
                {
                    // if (instance == null) throw new ArgumentNullExcpeiton("instance");
                    il.Ldarg_0();
                    il.Brtrue_S(args.Length > 0 ? lableCheckArgumentsRef : lableValidationCompleted);

                    il.ThrowArgumentsNullExcpetion("instance");
                }

                //check the arguments
                if (args.Length > 0)
                {
                    // if (arguments == null) throw new ArgumentNullExcpeiton("arguments");
                    il.MarkLabel(lableCheckArgumentsRef);
                    il.Ldarg_1();
                    il.Brtrue_S(lableCheckArgumentsLength);

                    il.ThrowArgumentsNullExcpetion("arguments");

                    // if (arguments.Length < $(args.Length)) throw new ArgumentExcpeiton(msg, "arguments");
                    il.MarkLabel(lableCheckArgumentsLength);
                    il.Ldarg_1();
                    il.Ldlen();
                    il.Conv_I4();
                    il.LoadInt32(args.Length);
                    il.Bge_S(lableValidationCompleted);

                    il.ThrowArgumentsExcpetion("Not enough arguments in the argument array.", "arguments");
                }
            }

            il.MarkLabel(lableValidationCompleted);
            if (!methodInfo.IsStatic)
            {
                il.Ldarg_0();
                il.CastReference(methodInfo.DeclaringType);
            }

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    il.Ldarg_1();
                    il.LoadInt32((short)i);
                    il.Ldelem_Ref();
                    il.CastValue(args[i].ParameterType);
                }
            }

            il.CallMethod(methodInfo);
            if (methodInfo.ReturnType == typeof(void))
            {
                il.Ldc_I4_0(); //return null
            }
            else
            {
                il.BoxIfNeeded(methodInfo.ReturnType);
            }
            il.Ret();

            var methodDelegate = dynamicMethod.CreateDelegate(typeof(Func<object, object[], object>));
            return (Func<object, object[], object>)methodDelegate;
        }
    }
}
