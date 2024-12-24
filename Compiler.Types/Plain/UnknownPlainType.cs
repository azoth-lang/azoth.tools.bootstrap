using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class UnknownPlainType : IMaybeFunctionPlainType
{
    #region Singleton
    internal static readonly UnknownPlainType Instance = new();

    private UnknownPlainType() { }
    #endregion

    IMaybePlainType IMaybeFunctionPlainType.Return => PlainType.Unknown;

    PlainTypeReplacements IMaybePlainType.TypeReplacements => PlainTypeReplacements.None;

    IMaybeNonVoidPlainType IMaybeNonVoidPlainType.ToNonLiteral() => PlainType.Unknown;

    #region Equality
    public bool Equals(IMaybePlainType? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        => obj is IMaybePlainType other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownPlainType));
    #endregion

    public override string ToString() => "⧼unknown⧽";
}
