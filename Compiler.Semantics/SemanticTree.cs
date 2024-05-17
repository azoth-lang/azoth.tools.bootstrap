using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public partial interface ITypeDefinitionNode
{
    IEnumerable<IStandardTypeNameNode> AllSupertypeNames => SupertypeNames;
}

public partial interface IClassDefinitionNode
{
    IEnumerable<IStandardTypeNameNode> ITypeDefinitionNode.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);
}

public partial interface ICapabilityNode
{
    // TODO some way to code gen this hiding
    ICapabilityConstraint ICapabilityConstraintNode.Constraint => Capability;
}

public partial interface IForeachExpressionNode
{
    IdentifierName IBindingDeclarationNode.Name => VariableName;
}

public partial interface IUserTypeDeclarationNode
{
    IEnumerable<ITypeMemberDeclarationNode> MembersNamed(StandardName named);
}

public partial interface INamespaceDeclarationNode
{
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named);
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named);
}
