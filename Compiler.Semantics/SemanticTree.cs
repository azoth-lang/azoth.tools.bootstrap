using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

#region Special Parts
public partial interface IBodyOrBlockNode
{
    LexicalScope GetContainingLexicalScope();
}
#endregion

#region Facets
public partial interface IPackageFacetNode
{
    // TODO some way to code gen this hiding
    PackageSymbol IPackageFacetDeclarationNode.Symbol => PackageSymbol;
}
#endregion

public partial interface ITypeDefinitionNode
{
    IEnumerable<IStandardTypeNameNode> AllSupertypeNames => SupertypeNames;
}

public partial interface IClassDefinitionNode
{
    IEnumerable<IStandardTypeNameNode> ITypeDefinitionNode.AllSupertypeNames
        => BaseTypeName is null ? SupertypeNames : SupertypeNames.Prepend(BaseTypeName);
}

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
    IPreviousValueId PreviousValueId();
    FlowState FlowStateBefore();
}

public partial interface INamedParameterNode
{
    new DataType BindingType { get; }
    new ValueId ValueId { get; }
    ValueId IParameterNode.ValueId => ValueId;
    ValueId IBindingNode.ValueId => ValueId;
}

public partial interface ISelfParameterNode
{
    new IMaybeAntetype BindingAntetype { get; }
    IMaybeAntetype IBindingNode.BindingAntetype => BindingAntetype;
    IMaybeAntetype IParameterNode.BindingAntetype => BindingAntetype;
    new Pseudotype BindingType { get; }
    Pseudotype IBindingNode.BindingType => BindingType;
    Pseudotype IParameterNode.BindingType => BindingType;
}
#endregion

#region Statements
public partial interface IStatementNode
{
    LexicalScope GetLexicalScope();
    IPreviousValueId PreviousValueId();
}

public partial interface IVariableDeclarationStatementNode
{
    FlowState FlowStateBefore();
}
#endregion

#region Patterns
public partial interface IPatternNode
{
    LexicalScope GetContainingLexicalScope();
    ConditionalLexicalScope GetFlowLexicalScope();
    IPreviousValueId PreviousValueId();
    IMaybeAntetype InheritedBindingAntetype();
}
#endregion

#region Expressions
public partial interface IAmbiguousExpressionNode
{
    LexicalScope GetContainingLexicalScope();
    // TODO it is strange that this is always a conditional scope. Instead, use conditional only where it makes sense?
    ConditionalLexicalScope GetFlowLexicalScope();
}

public partial interface IExpressionNode
{
    IPreviousValueId PreviousValueId();

    // TODO change inheritance so these are not expressions
    public bool ShouldNotBeExpression()
        => this is INamespaceNameNode or IFunctionGroupNameNode or IMethodGroupNameNode
            or IInitializerGroupNameNode or ITypeNameExpressionNode;
}

public partial interface IBlockExpressionNode
{
    new IMaybeAntetype Antetype { get; }
    // TODO this ought to have been generated
    IMaybeAntetype IBlockOrResultNode.Antetype => Antetype;
    IMaybeExpressionAntetype IExpressionNode.Antetype => Antetype;
    FlowState FlowStateBefore();
    new FlowState FlowStateAfter { get; }
}

public partial interface INewObjectExpressionNode
{
    PackageNameScope InheritedPackageNameScope();
    FlowState FlowStateBefore();
}
#endregion

#region Control Flow Expressions
public partial interface IIfExpressionNode
{
    new ValueId ValueId { get; }
    ValueId IElseClauseNode.ValueId => ValueId;
    ValueId IExpressionNode.ValueId => ValueId;
}

public partial interface IForeachExpressionNode
{
    // TODO some way to code gen this hiding
    IdentifierName INamedBindingDeclarationNode.Name => VariableName;

    PackageNameScope InheritedPackageNameScope();
}
#endregion

#region Invocation Expressions
public partial interface IFunctionInvocationExpressionNode
{
    FlowState FlowStateBefore();
}
#endregion
#region Ambiguous Name Expressions
public partial interface IMemberAccessExpressionNode
{
    PackageNameScope InheritedPackageNameScope();
}
#endregion

#region Name Expressions
public partial interface IVariableNameExpressionNode
{
    FlowState FlowStateBefore();
}

public partial interface ISelfExpressionNode
{
    FlowState FlowStateBefore();
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

