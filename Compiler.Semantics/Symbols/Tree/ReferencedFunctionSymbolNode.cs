using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class ReferencedFunctionSymbolNode : ReferencedDeclarationSymbolNode, IFunctionDeclarationNode
{
    public override StandardName Name => base.Name!;
    public override FunctionSymbol Symbol { get; }

    public ReferencedFunctionSymbolNode(FunctionSymbol symbol)
    {
        Symbol = symbol;
    }
}
