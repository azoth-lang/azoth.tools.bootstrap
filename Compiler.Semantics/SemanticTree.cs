using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

#region Facets
public partial interface IPackageFacetNode
{
    // TODO some way to code gen this hiding
    PackageSymbol IPackageFacetDeclarationNode.Symbol => PackageSymbol;
}
#endregion

#region Capabilities
public partial interface ICapabilityNode
{
    // TODO some way to code gen this hiding
    ICapabilityConstraint ICapabilityConstraintNode.Constraint => Capability;
}
#endregion

#region Patterns
public partial interface IPatternNode
{
    DataType ContextBindingType();
    ValueId? MatchReferentValueId { get; }
}
#endregion

#region Expressions
public partial interface IExpressionNode
{
    /// <summary>
    /// Indicates that this node type should not actually be counted as an expression. (i.e. it
    /// should implement <see cref="IExpressionNode"/>.
    /// </summary>
    // TODO change inheritance so these are not expressions
    public bool ShouldNotBeExpression()
        => this is INamespaceNameNode or IFunctionGroupNameNode or IMethodGroupNameNode
            or IInitializerGroupNameNode or ITypeNameExpressionNode;
}

public partial interface INewObjectExpressionNode
{
    PackageNameScope PackageNameScope();
}
#endregion

#region Control Flow Expressions
public partial interface IForeachExpressionNode
{
    // TODO some way to code gen this hiding
    IdentifierName INamedBindingDeclarationNode.Name => VariableName;
    PackageNameScope PackageNameScope();
}

public partial interface IReturnExpressionNode
{
    DataType? ExpectedReturnType { get; }
    IExitNode ControlFlowExit();
}
#endregion

#region Ambiguous Name Expressions
public partial interface IMemberAccessExpressionNode
{
    PackageNameScope PackageNameScope();
}
#endregion

#region Type Declarations
public partial interface ITypeDeclarationNode
{
    /// <summary>
    /// All members of the type with the given name along with inherited members with the given name
    /// if they are not hidden by a member in the type itself.
    /// </summary>
    IEnumerable<IInstanceMemberDeclarationNode> InclusiveInstanceMembersNamed(StandardName named);
    IEnumerable<IAssociatedMemberDeclarationNode> AssociatedMembersNamed(StandardName named);
}
#endregion

#region Namespace Declarations
public partial interface INamespaceDeclarationNode
{
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named);
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named);
}
#endregion
