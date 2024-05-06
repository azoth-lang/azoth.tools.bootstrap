using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class ReferencedTypeNode : ReferencedDeclarationSymbolNode, ITypeSymbolNode
{
    public override UserTypeSymbol Symbol { get; }
    public abstract IFixedList<ITypeMemberSymbolNode> Members { get; }

    private protected ReferencedTypeNode(UserTypeSymbol symbol)
    {
        Symbol = symbol;
    }

    public override IEnumerable<IDeclarationSymbolNode> MembersNamed(IdentifierName named)
        => throw new System.NotImplementedException();
}
