using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class OptionalPlainType : NonVoidPlainType
{
    /// <remarks>The optional type acts as a value type that may contain a reference and as such
    /// always has value semantics.</remarks>
    public TypeSemantics? Semantics => TypeSemantics.Value;
    public NonVoidPlainType Referent { get; }

    public OptionalPlainType(NonVoidPlainType referent)
    {
        Referent = referent;
    }

    public IMaybePlainType ReplaceTypeParametersIn(IMaybePlainType plainType)
        => plainType;

    #region Equality
    public bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalPlainType that
            && Referent.Equals(that.Referent);
    }

    public override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Referent);
    #endregion

    public override string ToString() => $"{Referent}?";
}
