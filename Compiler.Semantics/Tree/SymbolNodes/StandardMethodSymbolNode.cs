using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class StandardMethodSymbolNode : MethodSymbolNode, IStandardMethodSymbolNode
{
    internal StandardMethodSymbolNode(MethodSymbol symbol)
        : base(symbol)
    {
        Requires.That(symbol.Kind == MethodKind.Standard, nameof(symbol), "Must be a standard method symbol.");
    }
}
