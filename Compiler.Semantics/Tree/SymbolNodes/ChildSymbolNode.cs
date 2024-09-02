using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class ChildSymbolNode : ChildNode, IChildDeclarationNode
{
    public override ISyntax? Syntax => null;
    public abstract Symbol Symbol { get; }

    protected IEnumerable<IChildDeclarationNode> GetMembers()
        => GetMembers(Inherited_SymbolTree(GrammarAttribute.CurrentInheritanceContext()));

    protected IEnumerable<IChildDeclarationNode> GetMembers(ISymbolTree symbolTree)
        => symbolTree.GetChildrenOf(Symbol).Where(sym => sym is not GenericParameterTypeSymbol)
                     .Select(SymbolBinder.Symbol).WhereNotNull();
}
