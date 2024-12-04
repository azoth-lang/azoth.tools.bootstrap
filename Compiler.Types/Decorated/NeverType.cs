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

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => PlainType.ToString();

    public string ToILString() => PlainType.ToString();
}
