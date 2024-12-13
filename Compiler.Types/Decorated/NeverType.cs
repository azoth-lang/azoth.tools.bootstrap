using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class NeverType : INonVoidType
{
    #region Singleton
    internal static readonly NeverType Instance = new NeverType();

    private NeverType() { }
    #endregion

    public override NeverPlainType PlainType => NeverPlainType.Instance;

    public override TypeReplacements TypeReplacements => TypeReplacements.None;

    public override bool HasIndependentTypeArguments => false;

    #region Equality
    public override bool Equals(IMaybeType? other)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(NeverType));
    #endregion

    public override string ToSourceCodeString() => PlainType.ToString();

    public override string ToILString() => PlainType.ToString();
}
