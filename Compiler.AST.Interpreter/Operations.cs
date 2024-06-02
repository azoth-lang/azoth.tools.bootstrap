using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter;

internal static class Operations
{
    public static AzothValue Convert(this AzothValue value, DataType from, CapabilityType to, bool isOptional)
    {
        if (from is IntegerConstValueType)
        {
            if (to == DataType.Int) return AzothValue.Int(value.IntValue);
            if (to == DataType.UInt) return AzothValue.Int(value.IntValue);

            if (to == DataType.Int8) return AzothValue.I8((sbyte)value.IntValue);
            if (to == DataType.Byte) return AzothValue.Byte((byte)value.IntValue);
            if (to == DataType.Int16) return AzothValue.I16((short)value.IntValue);
            if (to == DataType.UInt16) return AzothValue.U16((ushort)value.IntValue);
            if (to == DataType.Int32) return AzothValue.I32((int)value.IntValue);
            if (to == DataType.UInt32) return AzothValue.U32((uint)value.IntValue);
            if (to == DataType.Int64) return AzothValue.I64((long)value.IntValue);
            if (to == DataType.UInt64) return AzothValue.U64((ulong)value.IntValue);
            if (to == DataType.Offset) return AzothValue.Offset((nint)(long)value.IntValue);
            if (to == DataType.Size) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (to == DataType.NInt) return AzothValue.NInt((nint)(long)value.IntValue);
            if (to == DataType.NUInt) return AzothValue.NUInt((nuint)(ulong)value.IntValue);
        }

        if (from is ValueType<BoolType> or BoolConstValueType)
        {
            if (to == DataType.Int8) return AzothValue.I8((sbyte)(value.BoolValue ? 1 : 0));
            if (to == DataType.Byte) return AzothValue.Byte((byte)(value.BoolValue ? 1 : 0));
            if (to == DataType.Int16) return AzothValue.I16((short)(value.BoolValue ? 1 : 0));
            if (to == DataType.UInt16) return AzothValue.U16((ushort)(value.BoolValue ? 1 : 0));
            if (to == DataType.Int32) return AzothValue.I32(value.BoolValue ? 1 : 0);
            if (to == DataType.UInt32) return AzothValue.U32((uint)(value.BoolValue ? 1 : 0));
            if (to == DataType.Int64) return AzothValue.I64(value.BoolValue ? 1 : 0);
            if (to == DataType.UInt64) return AzothValue.U64((uint)(value.BoolValue ? 1 : 0));
            if (to == DataType.Offset) return AzothValue.Offset(value.BoolValue ? 1 : 0);
            if (to == DataType.Size) return AzothValue.Size((nuint)(value.BoolValue ? 1 : 0));
            if (to == DataType.NInt) return AzothValue.NInt(value.BoolValue ? 1 : 0);
            if (to == DataType.NUInt) return AzothValue.NUInt((nuint)(value.BoolValue ? 1 : 0));
            if (to == DataType.Int) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (to == DataType.UInt) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (to == DataType.Bool) return value;
        }

        if (from == DataType.Byte)
        {
            if (to == DataType.Int16) return AzothValue.I16(value.ByteValue);
            if (to == DataType.UInt16) return AzothValue.U16(value.ByteValue);
            if (to == DataType.Int32) return AzothValue.I32(value.ByteValue);
            if (to == DataType.UInt32) return AzothValue.U32(value.ByteValue);
            if (to == DataType.Offset) return AzothValue.Offset(value.ByteValue);
            if (to == DataType.Size) return AzothValue.Size(value.ByteValue);
            if (to == DataType.NInt) return AzothValue.NInt(value.ByteValue);
            if (to == DataType.NUInt) return AzothValue.NUInt(value.ByteValue);
            if (to == DataType.Int) return AzothValue.Int(value.ByteValue);
            if (to == DataType.UInt) return AzothValue.Int(value.ByteValue);
        }

        if (from == DataType.Size)
        {
            if (to == DataType.Offset) return AzothValue.Offset((nint)value.SizeValue);
            if (to == DataType.Size) return AzothValue.Size(value.SizeValue);
            if (to == DataType.NInt) return AzothValue.NInt((nint)value.SizeValue);
            if (to == DataType.NUInt) return AzothValue.NUInt(value.SizeValue);
            if (to == DataType.Int) return AzothValue.Int(value.SizeValue);
            if (to == DataType.UInt) return AzothValue.Int(value.SizeValue);
        }

        if (from == DataType.Offset)
        {
            if (to == DataType.NInt) return AzothValue.NInt((nint)value.SizeValue);
            if (to == DataType.Int) return AzothValue.Int(value.OffsetValue);
        }

        if (from == DataType.Int || from == DataType.UInt)
        {
            if (to == DataType.Int) return value;

            var fromValue = value.IntValue;
            if (to is ValueType<FixedSizeIntegerType> { DeclaredType: var fixedSizeIntegerType })
            {
                var isSigned = fromValue.Sign < 0;
                if (isSigned && (!fixedSizeIntegerType.IsSigned
                                 || fromValue.GetBitLength() > fixedSizeIntegerType.Bits))
                {
                    if (isOptional) return AzothValue.None;
                    throw new Abort($"Cannot convert value {fromValue} to {to.ToILString()}");
                }
            }

            if (to == DataType.Int8) return AzothValue.I8((sbyte)value.IntValue);
            if (to == DataType.Byte) return AzothValue.Byte((byte)value.IntValue);
            if (to == DataType.Int16) return AzothValue.I16((short)value.IntValue);
            if (to == DataType.UInt16) return AzothValue.U16((ushort)value.IntValue);
            if (to == DataType.Int32) return AzothValue.I32((int)value.IntValue);
            if (to == DataType.UInt32) return AzothValue.U32((uint)value.IntValue);
            if (to == DataType.Int64) return AzothValue.I64((long)value.IntValue);
            if (to == DataType.UInt64) return AzothValue.U64((ulong)value.IntValue);
            if (to == DataType.Offset) return AzothValue.Offset((nint)(long)value.IntValue);
            if (to == DataType.Size) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (to == DataType.NInt) return AzothValue.NInt((nint)(long)value.IntValue);
            if (to == DataType.NUInt) return AzothValue.NUInt((nuint)(ulong)value.IntValue);
        }

        throw new NotImplementedException($"Conversion from {from.ToILString()} to {to.ToILString()}");
    }
}
