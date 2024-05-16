using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class ChildSymbolNode : ChildNode, IChildDeclarationNode
{
    public override ISyntax? Syntax => null;
    public abstract Symbol Symbol { get; }

    internal override ISymbolTree InheritedSymbolTree(IChildDeclarationNode caller, IChildDeclarationNode child)
        => Parent.InheritedSymbolTree(this, child);

    internal override IPackageFacetDeclarationNode InheritedFacet(IChildNode caller, IChildNode child)
        => Parent.InheritedFacet(this, child);

    protected IEnumerable<IChildDeclarationNode> GetMembers()
    {
        var symbolTree = Parent.InheritedSymbolTree(this, this);
        return symbolTree.GetChildrenOf(Symbol).Select(SymbolNodeAttributes.Symbol);
    }
}
