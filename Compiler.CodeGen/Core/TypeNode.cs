using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

[DebuggerDisplay("{" + nameof(ToString) + ",nq}")]
public sealed record TypeNode(Symbol Symbol, bool IsList, bool IsOptional)
{
    public override string ToString()
    {
        var type = Symbol.ToString();
        if (IsList) type += "*";
        if (IsOptional) type += "?";
        return type;
    }
}
