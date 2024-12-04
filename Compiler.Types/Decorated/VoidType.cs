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

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => PlainType.ToString();

    public string ToILString() => PlainType.ToString();
}
