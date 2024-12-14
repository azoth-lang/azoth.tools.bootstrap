using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public sealed class VoidType : Type
{
    #region Singleton
    internal static readonly VoidType Instance = new VoidType();

    private VoidType() { }
    #endregion

    public override VoidPlainType PlainType => VoidPlainType.Instance;

    public override bool HasIndependentTypeArguments => false;

    #region Equality
    public override bool Equals(IMaybeType? other)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, other);

    public override int GetHashCode() => HashCode.Combine(typeof(VoidType));
    #endregion

    public override string ToSourceCodeString() => PlainType.ToString();

    public override string ToILString() => PlainType.ToString();
}
