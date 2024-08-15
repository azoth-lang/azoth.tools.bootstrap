using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class FunctionSymbolNode : PackageFacetChildSymbolNode, IFunctionSymbolNode
{
    public override StandardName Name => base.Name!;
    TypeName INamedDeclarationNode.Name => Name;
    public override FunctionSymbol Symbol { get; }
    public FunctionType Type => Symbol.Type;

    public FunctionSymbolNode(FunctionSymbol symbol)
    {
        Symbol = symbol;
    }
}
