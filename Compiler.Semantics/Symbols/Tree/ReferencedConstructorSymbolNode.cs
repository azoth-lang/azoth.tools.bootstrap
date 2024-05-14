using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedConstructorSymbolNode : ReferencedDeclarationSymbolNode, IConstructorSymbolNode
{
    public override ConstructorSymbol Symbol { get; }
    public override IdentifierName? Name => Symbol.Name;

    public ReferencedConstructorSymbolNode(ConstructorSymbol symbol)
    {
        Symbol = symbol;
    }
}
