using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticNamespaceSymbolNode : SemanticDeclarationSymbolNode, INamespaceSymbolNode
{
    public override IdentifierName Name => Symbol.Name;
    public override NamespaceSymbol Symbol { get; }
    public IFixedList<INamespaceMemberSymbolNode> Members { get; }
    private MultiMapHashSet<StandardName, INamespaceMemberSymbolNode>? membersByName;

    public SemanticNamespaceSymbolNode(NamespaceSymbol symbol, IEnumerable<INamespaceMemberSymbolNode> members)
    {
        Symbol = symbol;
        Members = ChildList.CreateFixed(this, members);
    }

    public IEnumerable<INamespaceMemberSymbolNode> MembersNamed(IdentifierName named)
        => Members.MembersNamed(ref membersByName, named);
}
