using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

public partial interface IDeclarationSymbolNode
{
    IEnumerable<IDeclarationSymbolNode> MembersNamed(StandardName named);
}

public partial interface INamespaceSymbolNode
{
    new IEnumerable<INamespaceMemberSymbolNode> MembersNamed(StandardName named);
    IEnumerable<IDeclarationSymbolNode> IDeclarationSymbolNode.MembersNamed(StandardName named)
        => MembersNamed(named);

    IEnumerable<INamespaceMemberSymbolNode> NestedMembersNamed(StandardName named);
}
