using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

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

#region Parameters
public partial interface IParameterNode
{
    IFlowState FlowStateBefore();
}
#endregion

#region Control Flow
public partial interface IControlFlowNode
{
    IEntryNode ControlFlowEntry();
    /// <summary>
    /// The control flow nodes that follow this node based on the context.
    /// </summary>
    /// <remarks>This is an inherited property.</remarks>
    ControlFlowSet ControlFlowFollowing();
}

public partial interface IEntryNode
{
    FixedDictionary<IVariableBindingNode, int> VariableBindingsMap();
}
#endregion

#region Statements
public partial interface IVariableDeclarationStatementNode
{
    IFlowState FlowStateBefore();
}
#endregion

#region Patterns
public partial interface IPatternNode
{
    IMaybeAntetype InheritedBindingAntetype();
    DataType InheritedBindingType();
    ValueId? MatchReferentValueId { get; }
}

public partial interface IBindingPatternNode
{
    IFlowState FlowStateBefore();
}
#endregion

#region Expressions
public partial interface IExpressionNode
{
    /// <summary>
    /// Whether an implicit recovery (i.e. move or freeze) is allowed to covert this expression to
    /// the expected type.
    /// </summary>
    bool ImplicitRecoveryAllowed();

    /// <summary>
    /// Whether this expression should be prepared for return.
    /// </summary>
    bool ShouldPrepareToReturn();

    /// <summary>
    /// Indicates that this node type should not actually be counted as an expression. (i.e. it
    /// should implement <see cref="IExpressionNode"/>.
    /// </summary>
    // TODO change inheritance so these are not expressions
    public bool ShouldNotBeExpression()
        => this is INamespaceNameNode or IFunctionGroupNameNode or IMethodGroupNameNode
            or IInitializerGroupNameNode or ITypeNameExpressionNode;
}

public partial interface IBlockExpressionNode
{
    IFlowState FlowStateBefore();
}

public partial interface INewObjectExpressionNode
{
    PackageNameScope InheritedPackageNameScope();
    IFlowState FlowStateBefore();
}
#endregion

#region Literal Expressions
public partial interface ILiteralExpressionNode
{
    IFlowState FlowStateBefore();
}
#endregion

#region Control Flow Expressions
public partial interface IForeachExpressionNode
{
    // TODO some way to code gen this hiding
    IdentifierName INamedBindingDeclarationNode.Name => VariableName;
    PackageNameScope InheritedPackageNameScope();
}

public partial interface IReturnExpressionNode
{
    DataType? ExpectedReturnType { get; }
    IExitNode ControlFlowExit();
}
#endregion

#region Invocation Expressions
public partial interface IFunctionInvocationExpressionNode
{
    IFlowState FlowStateBefore();
}

public partial interface IInitializerInvocationExpressionNode
{
    IFlowState FlowStateBefore();
}
#endregion

#region Ambiguous Name Expressions
public partial interface IMemberAccessExpressionNode
{
    PackageNameScope InheritedPackageNameScope();
}
#endregion

#region Name Expressions
public partial interface IFunctionNameNode
{
    IFlowState FlowStateBefore();
}

public partial interface IVariableNameExpressionNode
{
    IFlowState FlowStateBefore();
}

public partial interface ISelfExpressionNode
{
    IFlowState FlowStateBefore();
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
