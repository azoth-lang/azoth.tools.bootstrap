using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class VoidType : IType
{
    #region Singleton
    internal static readonly VoidType Instance = new VoidType();

    private VoidType() { }
    #endregion

    public VoidPlainType PlainType => VoidPlainType.Instance;
    IPlainType IType.PlainType => PlainType;

    #region Equality
    public bool Equals(IMaybeType? other)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, obj);

    public override int GetHashCode() => HashCode.Combine(typeof(VoidType));
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => PlainType.ToString();

    public string ToILString() => PlainType.ToString();
}
