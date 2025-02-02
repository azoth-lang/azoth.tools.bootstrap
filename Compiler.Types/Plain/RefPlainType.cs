using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

public sealed class RefPlainType : NonVoidPlainType
{
    /// <summary>
    /// Create a ref type for the given type (e.g. `ref T` given `T`).
    /// </summary>
    /// <remarks>Unknown, `void` and `never` types are not changed.</remarks>
    [return: NotNullIfNotNull(nameof(plainType))]
    public static IMaybePlainType? Create(IMaybePlainType? plainType, bool isInternal, bool isMutableBinding)
        => plainType switch
        {
            null => null,
            UnknownPlainType or NeverPlainType or VoidPlainType => plainType,
            NonVoidPlainType t => new RefPlainType(isInternal, isMutableBinding, t),
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Create a ref type for the given type (e.g. `ref T` given `T`).
    /// </summary>
    /// <remarks>`void` and `never` types are not changed.</remarks>
    public static PlainType Create(PlainType plainType, bool isInternal, bool isMutableBinding)
        => plainType switch
        {
            NeverPlainType or VoidPlainType => plainType,
            NonVoidPlainType t => new RefPlainType(isInternal, isMutableBinding, t),
            _ => throw new UnreachableException(),
        };

    /// <remarks>A ref type acts as a value type. This prevents comparing if two refs refer to the
    /// same variable. However, it avoids an ambiguity where </remarks>
    public override TypeSemantics? Semantics => TypeSemantics.Value;

    public bool IsInternal { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public NonVoidPlainType Referent { [DebuggerStepThrough] get; }

    public RefPlainType(bool isInternal, bool isMutableBinding, NonVoidPlainType referent)
    {
        Referent = referent;
        IsInternal = isInternal;
        IsMutableBinding = isMutableBinding;
    }

    #region Equality
    public override bool Equals(IMaybePlainType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is RefPlainType that && Referent.Equals(that.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(typeof(RefPlainType), Referent);
    #endregion

    public override string ToString()
    {
        var kind = IsInternal ? "iref" : "ref";
        var binding = IsMutableBinding ? "var " : "";
        return $"{kind} {binding}{Referent}";
    }
}
