using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

public partial interface IUserTypeDeclarationNode
{
    IEnumerable<ITypeMemberDeclarationNode> MembersNamed(StandardName named);
}

public partial interface INamespaceDeclarationNode
{
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named);
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named);
}
