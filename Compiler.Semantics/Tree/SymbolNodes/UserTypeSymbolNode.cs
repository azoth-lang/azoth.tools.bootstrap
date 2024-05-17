using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class UserTypeSymbolNode : PackageFacetChildSymbolNode, IUserTypeDeclarationNode
{
    public override StandardName Name => base.Name!;
    public override UserTypeSymbol Symbol { get; }
    public abstract IFixedList<ITypeMemberDeclarationNode> Members { get; }

    private protected UserTypeSymbolNode(UserTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    public IEnumerable<ITypeMemberDeclarationNode> MembersNamed(StandardName named)
        => throw new System.NotImplementedException();
}
