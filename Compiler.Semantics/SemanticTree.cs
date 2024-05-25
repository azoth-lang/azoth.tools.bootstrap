using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public partial interface IBodyOrBlockNode
{
    LexicalScope GetContainingLexicalScope();
}

public partial interface IPackageFacetNode
{
    // TODO some way to code gen this hiding
    PackageSymbol IPackageFacetDeclarationNode.Symbol => PackageSymbol;
}

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

#region Statements
public partial interface IStatementNode
{
    LexicalScope GetLexicalScope();
}
#endregion

#region Patterns
public partial interface IPatternNode
{
    LexicalScope GetContainingLexicalScope();
    ConditionalLexicalScope GetFlowLexicalScope();
}
#endregion

public partial interface IAmbiguousExpressionNode
{
    LexicalScope GetContainingLexicalScope();
    // TODO it is strange that this is always a conditional scope. Instead use conditional only where it makes sense?
    ConditionalLexicalScope GetFlowLexicalScope();
}

public partial interface IForeachExpressionNode
{
    // TODO some way to code gen this hiding
    IdentifierName INamedBindingDeclarationNode.Name => VariableName;
}

public partial interface IUserTypeDeclarationNode
{
    IEnumerable<IInstanceMemberDeclarationNode> InstanceMembersNamed(StandardName named);
    IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named);
}

public partial interface INamespaceDeclarationNode
{
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named);
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named);
}
