using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

internal static class Operations
{
    public static AzothValue Convert(this AzothValue value, IExpressionType from, CapabilityType to, bool isOptional)
    {
        if (from is IntegerConstValueType)
        {
            if (to.Equals(IType.Int)) return AzothValue.Int(value.IntValue);
            if (to.Equals(IType.UInt)) return AzothValue.Int(value.IntValue);
            if (to.Equals(IType.Int8)) return AzothValue.I8((sbyte)value.IntValue);
            if (to.Equals(IType.Byte)) return AzothValue.Byte((byte)value.IntValue);
            if (to.Equals(IType.Int16)) return AzothValue.I16((short)value.IntValue);
            if (to.Equals(IType.UInt16)) return AzothValue.U16((ushort)value.IntValue);
            if (to.Equals(IType.Int32)) return AzothValue.I32((int)value.IntValue);
            if (to.Equals(IType.UInt32)) return AzothValue.U32((uint)value.IntValue);
            if (to.Equals(IType.Int64)) return AzothValue.I64((long)value.IntValue);
            if (to.Equals(IType.UInt64)) return AzothValue.U64((ulong)value.IntValue);
            if (to.Equals(IType.Offset)) return AzothValue.Offset((nint)(long)value.IntValue);
            if (to.Equals(IType.Size)) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (to.Equals(IType.NInt)) return AzothValue.NInt((nint)(long)value.IntValue);
            if (to.Equals(IType.NUInt)) return AzothValue.NUInt((nuint)(ulong)value.IntValue);
        }

        if (from is CapabilityType<BoolType> or BoolConstValueType)
        {
            if (to.Equals(IType.Int8)) return AzothValue.I8((sbyte)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.Byte)) return AzothValue.Byte((byte)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.Int16)) return AzothValue.I16((short)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.UInt16)) return AzothValue.U16((ushort)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.Int32)) return AzothValue.I32(value.BoolValue ? 1 : 0);
            if (to.Equals(IType.UInt32)) return AzothValue.U32((uint)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.Int64)) return AzothValue.I64(value.BoolValue ? 1 : 0);
            if (to.Equals(IType.UInt64)) return AzothValue.U64((uint)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.Offset)) return AzothValue.Offset(value.BoolValue ? 1 : 0);
            if (to.Equals(IType.Size)) return AzothValue.Size((nuint)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.NInt)) return AzothValue.NInt(value.BoolValue ? 1 : 0);
            if (to.Equals(IType.NUInt)) return AzothValue.NUInt((nuint)(value.BoolValue ? 1 : 0));
            if (to.Equals(IType.Int)) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (to.Equals(IType.UInt)) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (to.Equals(IType.Bool)) return value;
        }

        if (from.Equals(IType.Byte))
        {
            if (to.Equals(IType.Int16)) return AzothValue.I16(value.ByteValue);
            if (to.Equals(IType.UInt16)) return AzothValue.U16(value.ByteValue);
            if (to.Equals(IType.Int32)) return AzothValue.I32(value.ByteValue);
            if (to.Equals(IType.UInt32)) return AzothValue.U32(value.ByteValue);
            if (to.Equals(IType.Offset)) return AzothValue.Offset(value.ByteValue);
            if (to.Equals(IType.Size)) return AzothValue.Size(value.ByteValue);
            if (to.Equals(IType.NInt)) return AzothValue.NInt(value.ByteValue);
            if (to.Equals(IType.NUInt)) return AzothValue.NUInt(value.ByteValue);
            if (to.Equals(IType.Int)) return AzothValue.Int(value.ByteValue);
            if (to.Equals(IType.UInt)) return AzothValue.Int(value.ByteValue);
        }

        if (from.Equals(IType.Size))
        {
            if (to.Equals(IType.Offset)) return AzothValue.Offset((nint)value.SizeValue);
            if (to.Equals(IType.Size)) return AzothValue.Size(value.SizeValue);
            if (to.Equals(IType.NInt)) return AzothValue.NInt((nint)value.SizeValue);
            if (to.Equals(IType.NUInt)) return AzothValue.NUInt(value.SizeValue);
            if (to.Equals(IType.Int)) return AzothValue.Int(value.SizeValue);
            if (to.Equals(IType.UInt)) return AzothValue.Int(value.SizeValue);
        }

        if (from.Equals(IType.Offset))
        {
            if (to.Equals(IType.NInt)) return AzothValue.NInt((nint)value.SizeValue);
            if (to.Equals(IType.Int)) return AzothValue.Int(value.OffsetValue);
        }

        if (from.Equals(IType.Int) || from.Equals(IType.UInt))
        {
            if (to.Equals(IType.Int)) return value;

            var fromValue = value.IntValue;
            if (to is CapabilityType<FixedSizeIntegerType> { DeclaredType: var fixedSizeIntegerType })
            {
                var isSigned = fromValue.Sign < 0;
                if (isSigned && (!fixedSizeIntegerType.IsSigned
                                 || fromValue.GetBitLength() > fixedSizeIntegerType.Bits))
                {
                    if (isOptional) return AzothValue.None;
                    throw new Abort($"Cannot convert value {fromValue} to {to.ToILString()}");
                }
            }

            if (to.Equals(IType.Int8)) return AzothValue.I8((sbyte)value.IntValue);
            if (to.Equals(IType.Byte)) return AzothValue.Byte((byte)value.IntValue);
            if (to.Equals(IType.Int16)) return AzothValue.I16((short)value.IntValue);
            if (to.Equals(IType.UInt16)) return AzothValue.U16((ushort)value.IntValue);
            if (to.Equals(IType.Int32)) return AzothValue.I32((int)value.IntValue);
            if (to.Equals(IType.UInt32)) return AzothValue.U32((uint)value.IntValue);
            if (to.Equals(IType.Int64)) return AzothValue.I64((long)value.IntValue);
            if (to.Equals(IType.UInt64)) return AzothValue.U64((ulong)value.IntValue);
            if (to.Equals(IType.Offset)) return AzothValue.Offset((nint)(long)value.IntValue);
            if (to.Equals(IType.Size)) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (to.Equals(IType.NInt)) return AzothValue.NInt((nint)(long)value.IntValue);
            if (to.Equals(IType.NUInt)) return AzothValue.NUInt((nuint)(ulong)value.IntValue);
        }

        throw new NotImplementedException($"Conversion from {from.ToILString()} to {to.ToILString()}");
    }
}
