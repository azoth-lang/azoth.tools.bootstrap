using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticFunctionSymbolNode : SemanticChildSymbolNode, IFunctionSymbolNode
{
    private readonly IFunctionDeclarationNode node;
    public override FunctionSymbol Symbol => throw new NotImplementedException();

    public SemanticFunctionSymbolNode(IFunctionDeclarationNode node)
    {
        this.node = node;
    }
}
