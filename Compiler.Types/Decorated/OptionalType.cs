using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class OptionalType : INonVoidType
{
    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown and void types are not changed.</remarks>
    public static IMaybeType Create(IMaybeType referent)
        => referent switch
        {
            INonVoidType t => new OptionalType(t),
            UnknownType _ => IType.Unknown,
            VoidType _ => IType.Void,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    /// <summary>
    /// Create an optional type for the given type (i.e. `T?` given `T`).
    /// </summary>
    /// <remarks>Unknown type produces unknown type.</remarks>
    public static IMaybeNonVoidType Create(IMaybeNonVoidType referent)
        => referent switch
        {
            INonVoidType t => new OptionalType(t),
            UnknownType _ => IType.Unknown,
            _ => throw ExhaustiveMatch.Failed(referent),
        };

    public static OptionalType Create(INonVoidType referent) => new OptionalType(referent);

    public OptionalPlainType PlainType { get; }
    NonVoidPlainType INonVoidType.PlainType => PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    public INonVoidType Referent { get; }

    public TypeReplacements TypeReplacements => Referent.TypeReplacements;

    public bool HasIndependentTypeArguments => Referent.HasIndependentTypeArguments;

    /// <remarks>This constructor takes <paramref name="plainType"/> even though it is fully implied
    /// by the other parameters to avoid allocating duplicate <see cref="OptionalPlainType"/>s.</remarks>
    public OptionalType(OptionalPlainType plainType, INonVoidType referent)
    {
        Requires.That(plainType.Referent.Equals(referent.PlainType), nameof(referent),
            "Referent must match the plain type.");
        PlainType = plainType;
        Referent = referent;
    }

    public OptionalType(INonVoidType referent)
        : this(new(referent.PlainType), referent) { }

    #region Equality
    public bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is OptionalType otherType
               && Referent.Equals(otherType.Referent);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is OptionalType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Referent);
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => $"{Referent.ToSourceCodeString()}?";

    public string ToILString() => $"{Referent.ToILString()}?";
}
