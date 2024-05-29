using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;

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

#region Flow
public partial interface IFlowNode
{
    IFlowNode Predecessor();
    FlowState FlowStateBefore();
}
#endregion

#region Parameters
public partial interface IParameterNode
{
    IPreviousValueId PreviousValueId();
}
#endregion

#region Statements
public partial interface IStatementNode
{
    LexicalScope GetLexicalScope();
    /// <summary>
    /// The predecessor of the whole statement as given by the parent of this statement.
    /// </summary>
    /// <returns></returns>
    IFlowNode InheritedPredecessor();
    /// <summary>
    /// The flow node that executes before this statement completes in the flow.
    /// </summary>
    IFlowNode Predecessor();
    IPreviousValueId PreviousValueId();
}
#endregion

#region Patterns
public partial interface IPatternNode
{
    LexicalScope GetContainingLexicalScope();
    ConditionalLexicalScope GetFlowLexicalScope();
    IPreviousValueId PreviousValueId();
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
}
#endregion

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

#region Namespace Declarations
public partial interface INamespaceDeclarationNode
{
    IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named);
    IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named);
}
#endregion

