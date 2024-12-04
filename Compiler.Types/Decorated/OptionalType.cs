using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

[DebuggerDisplay("{" + nameof(ToILString) + "(),nq}")]
public sealed class OptionalType : INonVoidType
{
    public OptionalPlainType PlainType { get; }
    INonVoidPlainType INonVoidType.PlainType => PlainType;

    public IType Referent { get; }

    public OptionalType(OptionalPlainType plainType, IType referent)
    {
        Requires.That(referent.PlainType.Equals(plainType), nameof(referent),
            "Referent must match the plain type.");
        PlainType = plainType;
        Referent = referent;
    }

    public override string ToString() => throw new NotSupportedException();

    public string ToSourceCodeString() => $"{Referent.ToSourceCodeString()}?";

    public string ToILString() => $"{Referent.ToILString()}?";
}
