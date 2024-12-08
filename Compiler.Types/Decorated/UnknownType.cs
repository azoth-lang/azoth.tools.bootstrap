using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class UnknownType : IMaybeType
{
    #region Singleton
    internal static readonly UnknownType Instance = new UnknownType();

    private UnknownType() { }
    #endregion

    public UnknownPlainType PlainType => UnknownPlainType.Instance;
    IMaybePlainType IMaybeType.PlainType => PlainType;

    #region Equality
    public bool Equals(IMaybeType? other)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, other);

    public override bool Equals(object? obj)
        // Singleton, so a reference equality suffices
        => ReferenceEquals(this, obj);

    public override int GetHashCode() => HashCode.Combine(typeof(UnknownType));
    #endregion

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => PlainType.ToString();

    public string ToILString() => PlainType.ToString();
}
