using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

// e.g. T
// NOTE: generic parameters are the only plain types that do not need a capability
[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class GenericParameterType : INonVoidType
{
    public GenericParameterPlainType PlainType { get; }
    INonVoidPlainType INonVoidType.PlainType => PlainType;

    public GenericParameterType(GenericParameterPlainType plainType)
    {
        PlainType = plainType;
    }

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => PlainType.ToString();

    public string ToILString() => PlainType.ToString();
}
