using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed record TypeNode(SymbolNode Symbol, CollectionKind CollectionKind, bool IsOptional)
{
    public override string ToString()
    {
        var type = Symbol.ToString();
        switch (CollectionKind)
        {
            default:
                throw ExhaustiveMatch.Failed(CollectionKind);
            case CollectionKind.None:
                // Nothing
                break;
            case CollectionKind.List:
                type += "*";
                break;
            case CollectionKind.Set:
                type = $"{{{type}}}";
                break;
        }
        if (IsOptional) type += "?";
        return type;
    }
}
