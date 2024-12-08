using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class OptionalType : INonVoidType
{
    public OptionalPlainType PlainType { get; }
    INonVoidPlainType INonVoidType.PlainType => PlainType;

    public INonVoidType Referent { get; }

    /// <remarks>This constructor takes <paramref name="plainType"/> even though it is fully implied
    /// by the other parameters to avoid allocating duplicate <see cref="OptionalPlainType"/>s.</remarks>
    public OptionalType(OptionalPlainType plainType, INonVoidType referent)
    {
        Requires.That(plainType.Referent.Equals(referent.PlainType), nameof(referent),
            "Referent must match the plain type.");
        PlainType = plainType;
        Referent = referent;
    }

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
