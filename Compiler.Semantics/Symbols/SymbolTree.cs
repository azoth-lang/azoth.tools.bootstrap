using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

public partial interface IDeclarationSymbolNode
{
    IEnumerable<IDeclarationSymbolNode> MembersNamed(IdentifierName named);
}

public partial interface INamespaceSymbolNode
{
    new IEnumerable<INamespaceMemberSymbolNode> MembersNamed(IdentifierName named);
    IEnumerable<IDeclarationSymbolNode> IDeclarationSymbolNode.MembersNamed(IdentifierName named)
        => MembersNamed(named);
}
