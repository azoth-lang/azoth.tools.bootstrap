using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class GetterMethodSymbolNode : MethodSymbolNode, IGetterMethodSymbolNode
{
    internal GetterMethodSymbolNode(MethodSymbol symbol)
        : base(symbol)
    {
        Requires.That(symbol.Kind == MethodKind.Getter, nameof(symbol), "Must be a getter symbol.");
    }
}
