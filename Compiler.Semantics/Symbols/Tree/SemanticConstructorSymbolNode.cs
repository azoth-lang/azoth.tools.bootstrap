using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticConstructorSymbolNode : SemanticDeclarationSymbolNode, IConstructorSymbolNode
{
    private readonly IConstructorDeclarationNode node;
    public override IdentifierName? Name => node.Name;
    public override ConstructorSymbol Symbol => node.Symbol;

    public SemanticConstructorSymbolNode(IConstructorDeclarationNode node)
    {
        this.node = node;
    }
}
