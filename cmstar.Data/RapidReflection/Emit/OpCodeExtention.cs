using System;
using System.Reflection;
using System.Reflection.Emit;

namespace cmstar.Data.RapidReflection.Emit
{
    /// <summary>
    /// Provides a set of extention methods for <see cref="ILGenerator"/>
    /// for emitting the <see cref="OpCode"/>s.
    /// </summary>
    public static class OpCodeExtention
    {
        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than
        /// or equal to the second value.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Bge(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bge, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value 
        /// is greater than or equal to the second value.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Bge_S(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bge_S, label);
            return il;
        }

        /// <summary>
        /// Converts a value type to an object reference (type O).
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="type">The value type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Box(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Box, type);
            return il;
        }

        /// <summary>
        /// Unconditionally transfers control to a target instruction.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Br(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Br, label);
            return il;
        }

        /// <summary>
        /// Unconditionally transfers control to a target instruction (short form).
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Br_S(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Br_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if value is <c>false</c>, 
        /// a null reference, or zero. 
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Brfalse(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brfalse, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if value is <c>false</c>, 
        /// a null reference, or zero. 
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Brfalse_S(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brfalse_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if value is <c>true</c>, 
        /// not null, or non-zero.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Brtrue(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brtrue, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if value is <c>true</c>, 
        /// not null, or non-zero.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="label">The label.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Brtrue_S(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brtrue_S, label);
            return il;
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="methodInfo">The mechod to call.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Call(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Call, methodInfo);
            return il;
        }

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="methodInfo">The mechod to call.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Callvirt(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Callvirt, methodInfo);
            return il;
        }

        /// <summary>
        /// Attempts to cast an object passed by reference to the specified class.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="type">The target class.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Castclass(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Castclass, type);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to native int.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Conv_I(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Conv_I1(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I1);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Conv_I2(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I2);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Conv_I4(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I4);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int64.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Conv_I8(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I8);
            return il;
        }

        /// <summary>
        /// Initializes each field of the value type at a specified address to a null reference 
        /// or a 0 of the appropriate primitive type.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="type">The value type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Initobj(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Initobj, type);
            return il;
        }

        /// <summary>
        /// Loads the argument at index 0 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarg_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            return il;
        }

        /// <summary>
        /// Loads the argument at index 1 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarg_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_1);
            return il;
        }

        /// <summary>
        /// Loads the argument at index 2 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarg_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_2);
            return il;
        }

        /// <summary>
        /// Loads the argument at index 3 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarg_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_3);
            return il;
        }

        /// <summary>
        /// Loads the argument (referenced by a specified short form index) onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarg_S(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldarg_S, index);
            return il;
        }

        /// <summary>
        /// Loads an argument (referenced by a specified index value) onto the stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarg(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldarg, index);
            return il;
        }

        /// <summary>
        /// Load an argument address, in short form, onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarga_S(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldarga_S, index);
            return il;
        }

        /// <summary>
        /// Load an argument address onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldarga(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldarga, index);
            return il;
        }

        /// <summary>
        /// Finds the value of a field in the object whose reference is currently on the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="fieldInfo">The target field.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of -1 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_M1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_M1);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 0 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_0);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 1 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_1);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 2 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_2);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 3 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_3);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 4 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_4);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 5 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_5(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_5);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 6 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_6(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_6);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 7 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_7(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_7);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 8 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_8(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_8);
            return il;
        }

        /// <summary>
        /// Pushes the supplied int8 value onto the evaluation stack as an int32, short form.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="value">The value.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4_S(this ILGenerator il, byte value)
        {
            il.Emit(OpCodes.Ldc_I4_S, value);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="value">The value.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I4(this ILGenerator il, int value)
        {
            il.Emit(OpCodes.Ldc_I4, value);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type int64 onto the evaluation stack as an int64.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="value">The value.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_I8(this ILGenerator il, long value)
        {
            il.Emit(OpCodes.Ldc_I8, value);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type float32 onto the evaluation stack as type F (float).
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="value">The value.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_R4(this ILGenerator il, float value)
        {
            il.Emit(OpCodes.Ldc_R4, value);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type float64 onto the evaluation stack as type F (float).
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="value">The value.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldc_R8(this ILGenerator il, double value)
        {
            il.Emit(OpCodes.Ldc_R8, value);
            return il;
        }

        /// <summary>
        /// Loads the element containing an object reference at a specified array index onto 
        /// the top of the evaluation stack as type O (object reference).
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldelem_Ref(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_Ref);
            return il;
        }

        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldlen(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldlen);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 0 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_0);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 1 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_1);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 2 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_2);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 3 onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_3);
            return il;
        }

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack, short form.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc_S(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldloc_S, index);
            return il;
        }

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack, short form.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="localBuilder">The instance of <see cref="LocalBuilder"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc_S(this ILGenerator il, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Ldloc_S, localBuilder);
            return il;
        }

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldloc, index);
            return il;
        }

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="localBuilder">The instance of <see cref="LocalBuilder"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloc(this ILGenerator il, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Ldloc, localBuilder);
            return il;
        }

        /// <summary>
        /// Loads the address of the local variable at a specific index onto
        /// the evaluation stack, short form.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloca_S(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldloca_S, index);
            return il;
        }

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldloca(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldloca, index);
            return il;
        }

        /// <summary>
        /// Pushes the value of a static field onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="fieldInfo">The static field.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldsfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldsfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Pushes a new object reference to a string literal stored in the metadata.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="value">The string.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ldstr(this ILGenerator il, string value)
        {
            il.Emit(OpCodes.Ldstr, value);
            return il;
        }

        /// <summary>
        /// Creates a new object or a new instance of a value type, pushing an object reference (type O) 
        /// onto the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="constructorInfo">The constructor of the type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Newobj(this ILGenerator il, ConstructorInfo constructorInfo)
        {
            il.Emit(OpCodes.Newobj, constructorInfo);
            return il;
        }

        /// <summary>
        /// Returns from the current method, pushing a return value (if present) from 
        /// the callee's evaluation stack onto the caller's evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Ret(this ILGenerator il)
        {
            il.Emit(OpCodes.Ret);
            return il;
        }

        /// <summary>
        /// Replaces the value stored in the field of an object reference or pointer with a new value.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="fieldInfo">The target field.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Stfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at index 0.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_0);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at index 1.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_1);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at index 2.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_2);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at index 3.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_3);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at index (short form).
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc_S(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Stloc_S, index);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at index (short form).
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="localBuilder">The instance of <see cref="LocalBuilder"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc_S(this ILGenerator il, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Stloc_S, localBuilder);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at a specified index.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="index">The index.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Stloc, index);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in a the local variable list at a specified index.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="localBuilder">The instance of <see cref="LocalBuilder"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stloc(this ILGenerator il, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Stloc, localBuilder);
            return il;
        }

        /// <summary>
        /// Replaces the value of a static field with a value from the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="fieldInfo">The static field.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Stsfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Stsfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Throws the exception object currently on the evaluation stack.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Throw(this ILGenerator il)
        {
            il.Emit(OpCodes.Throw);
            return il;
        }

        /// <summary>
        /// Converts the boxed representation of a value type to its unboxed form.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="type">The value type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Unbox(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Unbox, type);
            return il;
        }

        /// <summary>
        /// Converts the boxed representation of a type specified 
        /// in the instruction to its unboxed form.
        /// </summary>
        /// <param name="il">The instance of <see cref="ILGenerator"/>.</param>
        /// <param name="type">The type.</param>
        /// <returns>The instance of <see cref="ILGenerator"/>.</returns>
        public static ILGenerator Unbox_Any(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Unbox_Any, type);
            return il;
        }
    }
}
