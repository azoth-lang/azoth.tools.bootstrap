using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class ChildSymbolNode : ChildNode, IChildDeclarationNode
{
    public override ISyntax? Syntax => null;
    public abstract Symbol Symbol { get; }

    protected IEnumerable<IChildDeclarationNode> GetMembers()
    {
        var symbolTree = Parent.InheritedSymbolTree(this, this);
        return GetMembers(symbolTree);
    }

    protected IEnumerable<IChildDeclarationNode> GetMembers(ISymbolTree symbolTree)
        => symbolTree.GetChildrenOf(Symbol).Select(SymbolNodeAspect.Symbol).WhereNotNull();
}
