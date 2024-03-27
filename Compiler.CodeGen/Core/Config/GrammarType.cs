using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

[DebuggerDisplay("{" + nameof(ToString) + ",nq}")]
public sealed record GrammarType(GrammarSymbol Symbol, bool IsRef, bool IsList, bool IsOptional)
{
    public override string ToString()
    {
        var type = Symbol.ToString();
        if (IsRef) type = "&" + type;
        if (IsList) type += "*";
        if (IsOptional) type += "?";
        return type;
    }
}
