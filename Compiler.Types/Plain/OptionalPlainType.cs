using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class OptionalPlainType : NonVoidPlainType
{
    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown and `void` types are not changed.</remarks>
    public static IMaybePlainType Create(IMaybePlainType plainType)
        => plainType switch
        {
            UnknownPlainType or VoidPlainType => plainType,
            NonVoidPlainType t => new OptionalPlainType(t),
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>`void` types are not changed.</remarks>
    [return: NotNullIfNotNull(nameof(plainType))]
    public static PlainType? Create(PlainType? plainType)
        => plainType switch
        {
            null => null,
            VoidPlainType => plainType,
            NonVoidPlainType t => new OptionalPlainType(t),
            _ => throw new UnreachableException(),
        };

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

    public override int GetHashCode() => HashCode.Combine(typeof(OptionalPlainType), Referent);
    #endregion

    public override string ToString() => $"{Referent}?";
}
