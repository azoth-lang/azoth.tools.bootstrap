using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedTypeDeclarationNode : ReferencedDeclarationSymbolNode, ITypeDeclarationSymbolNode
{
    public override UserTypeSymbol Symbol { get; }
    public abstract IFixedList<ITypeMemberSymbolNode> Members { get; }

    private protected ReferencedTypeDeclarationNode(UserTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    public override IEnumerable<IDeclarationSymbolNode> MembersNamed(StandardName named)
        => throw new System.NotImplementedException();
}
