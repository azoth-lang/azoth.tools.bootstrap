using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter;

internal static class Operations
{
    public static AzothValue Convert(this AzothValue value, DataType from, NumericType to)
    {
        if (from is IntegerConstantType)
        {
            if (to == DataType.Byte) return AzothValue.Byte((byte)value.IntValue);
            if (to == DataType.Int32) return AzothValue.I32((int)value.IntValue);
            if (to == DataType.UInt32) return AzothValue.U32((uint)value.IntValue);
            if (to == DataType.Offset) return AzothValue.Offset((nint)(long)value.IntValue);
            if (to == DataType.Size) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (to == DataType.Int) return AzothValue.Int(value.IntValue);
            if (to == DataType.UInt) return AzothValue.Int(value.IntValue);
        }

        if (from is BoolType)
        {
            if (to == DataType.Byte) return AzothValue.Byte((byte)(value.BoolValue ? 1 : 0));
            if (to == DataType.Int32) return AzothValue.I32(value.BoolValue ? 1 : 0);
            if (to == DataType.UInt32) return AzothValue.U32((uint)(value.BoolValue ? 1 : 0));
            if (to == DataType.Offset) return AzothValue.Offset(value.BoolValue ? 1 : 0);
            if (to == DataType.Size) return AzothValue.Size((nuint)(value.BoolValue ? 1 : 0));
            if (to == DataType.Int) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (to == DataType.UInt) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
        }

        if (from == DataType.Byte)
        {
            if (to == DataType.Int32) return AzothValue.I32(value.ByteValue);
            if (to == DataType.UInt32) return AzothValue.U32(value.ByteValue);
            if (to == DataType.Offset) return AzothValue.Offset(value.ByteValue);
            if (to == DataType.Size) return AzothValue.Size(value.ByteValue);
            if (to == DataType.Int) return AzothValue.Int(value.ByteValue);
            if (to == DataType.UInt) return AzothValue.Int(value.ByteValue);
        }

        if (from == DataType.Size)
        {
            if (to == DataType.Offset) return AzothValue.Offset((nint)value.SizeValue);
            if (to == DataType.Size) return AzothValue.Size(value.SizeValue);
            if (to == DataType.Int) return AzothValue.Int(value.SizeValue);
            if (to == DataType.UInt) return AzothValue.Int(value.SizeValue);
        }

        if (from == DataType.Offset)
        {
            if (to == DataType.Int) return AzothValue.Int(value.OffsetValue);
        }

        if (from == DataType.UInt)
            if (to == DataType.Int) return value;

        throw new NotImplementedException($"Conversion from {from.ToILString()} to {to.ToILString()}");
    }
}
