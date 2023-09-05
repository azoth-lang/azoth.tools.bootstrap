using System;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public sealed class FixedSizeIntegerType : IntegerType
{
    //internal new static readonly FixedSizeIntegerType Int8 = new("int8", -8);
    internal new static readonly FixedSizeIntegerType Byte = new(SpecialTypeName.Byte, 8);
    //internal new static readonly FixedSizeIntegerType Int16 = new("int16", -16);
    //internal new static readonly FixedSizeIntegerType UInt16 = new("uint16", 16);
    internal new static readonly FixedSizeIntegerType Int32 = new(SpecialTypeName.Int, -32);
    internal new static readonly FixedSizeIntegerType UInt32 = new(SpecialTypeName.UInt, 32);
    //internal new static readonly FixedSizeIntegerType Int64 = new("int64", -64);
    //internal new static readonly FixedSizeIntegerType UInt64 = new("uint64", 64);

    public int Bits { get; }
    public override bool IsKnown => true;

    private FixedSizeIntegerType(SpecialTypeName name, int bits)
        : base(name, bits < 0)
    {
        Bits = Math.Abs(bits);
    }
}
