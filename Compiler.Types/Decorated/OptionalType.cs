using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <remarks>Even though optional types act like a value type that is declared `const`, they have
/// a unique subtype relationship so they aren't just a special value type (i.e. `T &lt;: T?`).</remarks>
public sealed class OptionalType : NonVoidType
{
    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown and void types are not changed.</remarks>
    public static IMaybeType Create(IMaybeType referent)
        => referent switch
        {
            NonVoidType t => new OptionalType(t),
            UnknownType _ => Unknown,
            VoidType _ => Void,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Void types are not changed.</remarks>
    public static Type Create(Type referent)
        => referent switch
        {
            NonVoidType t => new OptionalType(t),
            VoidType _ => Void,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown type produces unknown type.</remarks>
    public static IMaybeNonVoidType Create(IMaybeNonVoidType referent)
        => referent switch
        {
            NonVoidType t => new OptionalType(t),
            UnknownType _ => Type.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    [DebuggerStepThrough]
    public static OptionalType Create(NonVoidType referent) => new OptionalType(referent);

    public override OptionalPlainType PlainType { [DebuggerStepThrough] get; }

    public NonVoidType Referent { [DebuggerStepThrough] get; }

    internal override GenericParameterTypeReplacements BareTypeReplacements => Referent.BareTypeReplacements;

    public override bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    /// <remarks>This constructor takes <paramref name="plainType"/> even though it is fully implied
    /// by the other parameters to avoid allocating duplicate <see cref="OptionalPlainType"/>s.</remarks>
    public OptionalType(OptionalPlainType plainType, NonVoidType referent)
    {
        Requires.That(plainType.Referent.Equals(referent.PlainType), nameof(referent),
            "Referent must match the plain type.");
        PlainType = plainType;
        Referent = referent;
    }

    public OptionalType(NonVoidType referent)
        : this(new(referent.PlainType), referent) { }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalType otherType
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Referent);
    #endregion

    public override string ToSourceCodeString() => $"{Referent.ToSourceCodeString()}?";

    public override string ToILString() => $"{Referent.ToILString()}?";
}
