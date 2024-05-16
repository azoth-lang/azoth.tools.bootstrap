using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticGenericParameterDeclarationNode : SemanticChildSymbolNode, IGenericParameterSymbolNode
{
    private readonly IGenericParameterNode node;
    public override GenericParameterTypeSymbol Symbol => node.Symbol;
    TypeSymbol ITypeDeclarationNode.Symbol => Symbol;
    public IdentifierName Name => node.Name;

    public SemanticGenericParameterDeclarationNode(IGenericParameterNode node)
    {
        this.node = node;
    }
}
