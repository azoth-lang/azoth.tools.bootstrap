using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

internal static class Operations
{
    public static AzothValue Convert(this AzothValue value, Type from, CapabilityType to, bool isOptional)
    {
        if (from is CapabilityType { TypeConstructor: IntegerLiteralTypeConstructor })
        {
            if (to.Equals(Type.Int)) return AzothValue.Int(value.IntValue);
            if (to.Equals(Type.UInt)) return AzothValue.Int(value.IntValue);
            if (to.Equals(Type.Int8)) return AzothValue.I8((sbyte)value.IntValue);
            if (to.Equals(Type.Byte)) return AzothValue.Byte((byte)value.IntValue);
            if (to.Equals(Type.Int16)) return AzothValue.I16((short)value.IntValue);
            if (to.Equals(Type.UInt16)) return AzothValue.U16((ushort)value.IntValue);
            if (to.Equals(Type.Int32)) return AzothValue.I32((int)value.IntValue);
            if (to.Equals(Type.UInt32)) return AzothValue.U32((uint)value.IntValue);
            if (to.Equals(Type.Int64)) return AzothValue.I64((long)value.IntValue);
            if (to.Equals(Type.UInt64)) return AzothValue.U64((ulong)value.IntValue);
            if (to.Equals(Type.Offset)) return AzothValue.Offset((nint)(long)value.IntValue);
            if (to.Equals(Type.Size)) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (to.Equals(Type.NInt)) return AzothValue.NInt((nint)(long)value.IntValue);
            if (to.Equals(Type.NUInt)) return AzothValue.NUInt((nuint)(ulong)value.IntValue);
        }

        if (from is CapabilityType { TypeConstructor: BoolTypeConstructor or BoolLiteralTypeConstructor })
        {
            if (to.Equals(Type.Int8)) return AzothValue.I8((sbyte)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.Byte)) return AzothValue.Byte((byte)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.Int16)) return AzothValue.I16((short)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.UInt16)) return AzothValue.U16((ushort)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.Int32)) return AzothValue.I32(value.BoolValue ? 1 : 0);
            if (to.Equals(Type.UInt32)) return AzothValue.U32((uint)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.Int64)) return AzothValue.I64(value.BoolValue ? 1 : 0);
            if (to.Equals(Type.UInt64)) return AzothValue.U64((uint)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.Offset)) return AzothValue.Offset(value.BoolValue ? 1 : 0);
            if (to.Equals(Type.Size)) return AzothValue.Size((nuint)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.NInt)) return AzothValue.NInt(value.BoolValue ? 1 : 0);
            if (to.Equals(Type.NUInt)) return AzothValue.NUInt((nuint)(value.BoolValue ? 1 : 0));
            if (to.Equals(Type.Int)) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (to.Equals(Type.UInt)) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (to.Equals(Type.Bool)) return value;
        }

        if (from.Equals(Type.Byte))
        {
            if (to.Equals(Type.Int16)) return AzothValue.I16(value.ByteValue);
            if (to.Equals(Type.UInt16)) return AzothValue.U16(value.ByteValue);
            if (to.Equals(Type.Int32)) return AzothValue.I32(value.ByteValue);
            if (to.Equals(Type.UInt32)) return AzothValue.U32(value.ByteValue);
            if (to.Equals(Type.Offset)) return AzothValue.Offset(value.ByteValue);
            if (to.Equals(Type.Size)) return AzothValue.Size(value.ByteValue);
            if (to.Equals(Type.NInt)) return AzothValue.NInt(value.ByteValue);
            if (to.Equals(Type.NUInt)) return AzothValue.NUInt(value.ByteValue);
            if (to.Equals(Type.Int)) return AzothValue.Int(value.ByteValue);
            if (to.Equals(Type.UInt)) return AzothValue.Int(value.ByteValue);
        }

        if (from.Equals(Type.Size))
        {
            if (to.Equals(Type.Offset)) return AzothValue.Offset((nint)value.SizeValue);
            if (to.Equals(Type.Size)) return AzothValue.Size(value.SizeValue);
            if (to.Equals(Type.NInt)) return AzothValue.NInt((nint)value.SizeValue);
            if (to.Equals(Type.NUInt)) return AzothValue.NUInt(value.SizeValue);
            if (to.Equals(Type.Int)) return AzothValue.Int(value.SizeValue);
            if (to.Equals(Type.UInt)) return AzothValue.Int(value.SizeValue);
        }

        if (from.Equals(Type.Offset))
        {
            if (to.Equals(Type.NInt)) return AzothValue.NInt((nint)value.SizeValue);
            if (to.Equals(Type.Int)) return AzothValue.Int(value.OffsetValue);
        }

        if (from.Equals(Type.Int) || from.Equals(Type.UInt))
        {
            if (to.Equals(Type.Int)) return value;

            var fromValue = value.IntValue;
            if (to is { TypeConstructor: FixedSizeIntegerTypeConstructor fixedSizeIntegerType })
            {
                var isSigned = fromValue.Sign < 0;
                if (isSigned && (!fixedSizeIntegerType.IsSigned
                                 || fromValue.GetBitLength() > fixedSizeIntegerType.Bits))
                {
                    if (isOptional) return AzothValue.None;
                    throw new Abort($"Cannot convert value {fromValue} to {to.ToILString()}");
                }
            }

            if (to.Equals(Type.Int8)) return AzothValue.I8((sbyte)value.IntValue);
            if (to.Equals(Type.Byte)) return AzothValue.Byte((byte)value.IntValue);
            if (to.Equals(Type.Int16)) return AzothValue.I16((short)value.IntValue);
            if (to.Equals(Type.UInt16)) return AzothValue.U16((ushort)value.IntValue);
            if (to.Equals(Type.Int32)) return AzothValue.I32((int)value.IntValue);
            if (to.Equals(Type.UInt32)) return AzothValue.U32((uint)value.IntValue);
            if (to.Equals(Type.Int64)) return AzothValue.I64((long)value.IntValue);
            if (to.Equals(Type.UInt64)) return AzothValue.U64((ulong)value.IntValue);
            if (to.Equals(Type.Offset)) return AzothValue.Offset((nint)(long)value.IntValue);
            if (to.Equals(Type.Size)) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (to.Equals(Type.NInt)) return AzothValue.NInt((nint)(long)value.IntValue);
            if (to.Equals(Type.NUInt)) return AzothValue.NUInt((nuint)(ulong)value.IntValue);
        }

        throw new NotImplementedException($"Conversion from {from.ToILString()} to {to.ToILString()}");
    }
}
