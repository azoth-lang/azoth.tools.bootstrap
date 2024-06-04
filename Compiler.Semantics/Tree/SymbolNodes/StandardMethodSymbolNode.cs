using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class StandardMethodSymbolNode : MethodSymbolNode, IStandardMethodSymbolNode
{
    internal StandardMethodSymbolNode(MethodSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.Kind == MethodKind.Standard, "Must be a standard method symbol.");
    }
}
