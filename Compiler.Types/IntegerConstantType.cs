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
    public override bool IsTypeOfConstant => true;
    public BigInteger Value { get; }
    public override bool IsFullyKnown => true;

    public IntegerConstantType(BigInteger value)
        : base(SpecialTypeName.ConstInt, value < BigInteger.Zero)
    {
        Value = value;
    }

    /// <summary>
    /// The default non-constant type to places values of this type in. For
    /// <see cref="IntegerConstantType"/>, that is <see cref="DataType.Int"/>.
    /// </summary>
    /// <remarks>It might be thought this should return the smallest integer type that contains
    /// the value. However, that would lead to unexpected behavior in some cases because small
    /// integer constants might produce small fixed size integers leading to overflow.</remarks>
    public override DataType ToNonConstantType() => Int;

    #region Equals
    public override bool Equals(DataType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerConstantType otherType
               && Value == otherType.Value;
    }

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToSourceCodeString()
        => throw new InvalidOperationException("Integer constant type has no source code representation");

    public override string ToILString() => $"const[{Value}]";
}
