using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticMethodSymbolNode : SemanticDeclarationSymbolNode, IMethodSymbolNode
{
    private readonly IMethodDefinitionNode node;
    public override MethodSymbol Symbol => node.Symbol;
    public override IdentifierName Name => node.Name;
    StandardName INamedSymbolNode.Name => Name;

    public SemanticMethodSymbolNode(IMethodDefinitionNode node)
    {
        this.node = node;
    }
}
