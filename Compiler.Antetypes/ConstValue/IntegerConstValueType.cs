using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;

public sealed class IntegerConstValueAntetype : ConstValueAntetype
{
    public BigInteger Value { get; }

    public IntegerConstValueAntetype(BigInteger value)
        : base(SpecialTypeName.ConstInt)
    {
        Value = value;
    }

    /// <summary>
    /// The default non-constant type to places values of this type in. For
    /// <see cref="IntegerConstValueAntetype"/>, that is <see cref="IAntetype.Int"/>.
    /// </summary>
    /// <remarks>It might be thought this should return the smallest integer type that contains
    /// the value. However, that would lead to unexpected behavior in some cases because small
    /// integer constants might produce small fixed size integers leading to overflow.</remarks>
    public override IAntetype ToNonConstValueType() => IAntetype.Int;

    #region Equality
    public override bool Equals(IMaybeExpressionAntetype? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is IntegerConstValueAntetype that
               && Value.Equals(that.Value);
    }

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToString() => $"Value[{Value}]";
}
