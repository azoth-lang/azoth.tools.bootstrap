using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticConstructorSymbolNode : SemanticDeclarationSymbolNode, IConstructorDeclarationNode
{
    private readonly IConstructorDefinitionNode node;
    public override IdentifierName? Name => node.Name;
    public override ConstructorSymbol Symbol => node.Symbol;

    public SemanticConstructorSymbolNode(IConstructorDefinitionNode node)
    {
        this.node = node;
    }
}
