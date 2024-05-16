using System;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticFunctionSymbolNode : SemanticDeclarationSymbolNode, IFunctionSymbolNode
{
    private IFunctionDefinitionNode Node { get; }
    public override StandardName Name => Node.Name;
    public override FunctionSymbol Symbol => throw new NotImplementedException();

    public SemanticFunctionSymbolNode(IFunctionDefinitionNode node)
    {
        Node = node;
    }
}
