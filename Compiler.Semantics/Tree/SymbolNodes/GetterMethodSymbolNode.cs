using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class GetterMethodSymbolNode : MethodSymbolNode, IGetterMethodSymbolNode
{
    internal GetterMethodSymbolNode(MethodSymbol symbol)
        : base(symbol)
    {
        Requires.That(nameof(symbol), symbol.Kind == MethodKind.Getter, "Must be a getter symbol.");
    }
}
