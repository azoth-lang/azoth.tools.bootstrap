using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticAssociatedFunctionDeclarationNode : SemanticDeclarationSymbolNode, IAssociatedFunctionSymbolNode
{
    private readonly IAssociatedFunctionDefinitionNode node;
    public override IdentifierName Name => node.Name;
    public override FunctionSymbol Symbol => node.Symbol;
    public FunctionType Type => node.Type;

    public SemanticAssociatedFunctionDeclarationNode(IAssociatedFunctionDefinitionNode node)
    {
        this.node = node;
    }
}
