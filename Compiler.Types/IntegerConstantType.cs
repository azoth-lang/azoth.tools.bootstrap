using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

/// <summary>
/// This is the type of integer constants, it isn't possible to declare a
/// variable to have this type.
/// </summary>
public sealed class IntegerConstantType : IntegerType
{
    public override bool IsConstant => true;
    public BigInteger Value { get; }
    public override bool IsKnown => true;

    public IntegerConstantType(BigInteger value)
        : base(SpecialTypeName.ConstInt, value < BigInteger.Zero)
    {
        Value = value;
    }

    public override DataType ToNonConstantType() => Int32;

    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerConstantType otherType
               && Value == otherType.Value;
    }

    public override int GetHashCode() => HashCode.Combine(Value);

    public override string ToSourceCodeString()
        => throw new InvalidOperationException("Integer constant type has no source code representation");

    public override string ToILString() => $"const[{Value}]";
}
