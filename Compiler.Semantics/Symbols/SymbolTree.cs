using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

public partial interface IUserTypeSymbolNode
{
    IEnumerable<ITypeMemberSymbolNode> MembersNamed(StandardName named);
}

public partial interface INamespaceSymbolNode
{
    IEnumerable<INamespaceMemberSymbolNode> MembersNamed(StandardName named);
    IEnumerable<INamespaceMemberSymbolNode> NestedMembersNamed(StandardName named);
}
