using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class SetterMethodSymbolNode : MethodSymbolNode, ISetterMethodSymbolNode
{
    internal SetterMethodSymbolNode(MethodSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.Kind == MethodKind.Setter, "Must be a standard method symbol.");
    }
}