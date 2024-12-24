using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class OptionalPlainType : NonVoidPlainType
{
    /// <remarks>The optional type acts as a value type that may contain a reference and as such
    /// always has value semantics.</remarks>
    public override TypeSemantics? Semantics => TypeSemantics.Value;
    public NonVoidPlainType Referent { [DebuggerStepThrough] get; }

    public OptionalPlainType(NonVoidPlainType referent)
    {
        Referent = referent;
    }

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalPlainType that
            && Referent.Equals(that.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Referent);
    #endregion

    public override string ToString() => $"{Referent}?";
}
