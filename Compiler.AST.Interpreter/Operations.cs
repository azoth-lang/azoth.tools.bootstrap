using System;
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

        throw new NotImplementedException($"Conversion from {from.ToILString()} to {to.ToILString()}");
    }
}
