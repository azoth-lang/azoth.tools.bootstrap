using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class RefType : NonVoidType
{
    public static IMaybeType Create(IMaybePlainType plainType, IMaybeType referent)
        => (plainType, referent) switch
        {
            (RefPlainType t, NonVoidType r) => new RefType(t, r),
            (VoidPlainType, VoidType) => Void,
            (NeverPlainType, NeverType) => Never,
            (UnknownPlainType, UnknownType) => Unknown,
            _ => throw new ArgumentException($"Plain type '{plainType}' does not match referent type '{referent}'."),
        };

    public static NonVoidType Create(IMaybePlainType plainType, NonVoidType referent)
        => (plainType, referent) switch
        {
            (RefPlainType t, NonVoidType r) => new RefType(t, r),
            (NeverPlainType, NeverType) => Never,
            _ => throw new ArgumentException($"Plain type '{plainType}' does not match referent type '{referent}'."),
        };

    public static RefType Create(RefPlainType plainType, NonVoidType referent)
        => new(plainType, referent);

    [return: NotNullIfNotNull(nameof(referent))]
    public static IMaybeType? CreateWithoutPlainType(bool isInternal, bool isMutableBinding, IMaybeType? referent)
        => referent switch
        {
            null => null,
            UnknownType _ => Unknown,
            VoidType _ => Void,
            NeverType _ => Never,
            NonVoidType t => new RefType(new(isInternal, isMutableBinding, t.PlainType), t),
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    /// <summary>
    /// Create a ref type for the given type (e.g. `ref T` given `T`).
    /// </summary>
    /// <remarks>`void` and `never` types are not changed.</remarks>
    public static Type CreateWithoutPlainType(bool isInternal, bool isMutableBinding, Type referent)
        => referent switch
        {
            VoidType _ => Void,
            NeverType _ => Never,
            NonVoidType t => new RefType(new(isInternal, isMutableBinding, t.PlainType), t),
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public override RefPlainType PlainType { [DebuggerStepThrough] get; }

    public bool IsInternal
    {
        [DebuggerStepThrough]
        get => PlainType.IsInternal;
    }

    public bool IsMutableBinding
    {
        [DebuggerStepThrough]
        get => PlainType.IsMutableBinding;
    }

    public NonVoidType Referent { [DebuggerStepThrough] get; }

    internal override GenericParameterTypeReplacements BareTypeReplacements => Referent.BareTypeReplacements;

    public override bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    private RefType(RefPlainType plainType, NonVoidType referent)
    {
        Requires.That(plainType.Referent.Equals(referent.PlainType), nameof(referent),
            "Referent must match the plain type.");
        PlainType = plainType;
        Referent = referent;
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is RefType otherType
               && IsInternal == otherType.IsInternal
               && IsMutableBinding == otherType.IsMutableBinding
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(IsInternal, IsMutableBinding, Referent);
    #endregion

    public override string ToSourceCodeString()
    {
        var kind = IsInternal ? "iref" : "ref";
        var binding = IsMutableBinding ? "var " : "";
        return $"{kind} {binding}{Referent.ToSourceCodeString()}";
    }

    public override string ToILString()
    {
        var kind = IsInternal ? "iref" : "ref";
        var binding = IsMutableBinding ? "var " : "";
        return $"{kind} {binding}{Referent.ToILString()}";
    }
}
