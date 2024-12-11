using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class NeverType : INonVoidType
{
    #region Singleton
    internal static readonly NeverType Instance = new NeverType();

    private NeverType() { }
    #endregion

    public NeverPlainType PlainType => NeverPlainType.Instance;
    INonVoidPlainType INonVoidType.PlainType => PlainType;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    public TypeReplacements TypeReplacements => TypeReplacements.None;

    public bool HasIndependentTypeArguments => false;

    #region Equality
    public bool Equals(IMaybeType? other)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, obj);

    public override int GetHashCode() => HashCode.Combine(typeof(NeverType));
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => PlainType.ToString();

    public string ToILString() => PlainType.ToString();
}
