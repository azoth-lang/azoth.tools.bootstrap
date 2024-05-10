using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticGenericParameterSymbolNode : SemanticChildSymbolNode, IGenericParameterSymbolNode
{
    private readonly IGenericParameterNode node;
    public override GenericParameterTypeSymbol Symbol => node.Symbol;
    TypeSymbol ITypeSymbolNode.Symbol => Symbol;
    public IdentifierName Name => node.Name;

    public SemanticGenericParameterSymbolNode(IGenericParameterNode node)
    {
        this.node = node;
    }
}
