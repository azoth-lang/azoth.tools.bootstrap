using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

#region Capabilities
public partial interface ICapabilityConstraintNode
{
    public ICapabilityConstraint ToConstraint(ICapabilityConstraint defaultConstraint)
        => this switch
        {
            ICapabilitySetNode n => n.CapabilitySet,
            ICapabilityNode n => n.DeclaredCapability.ToCapability(null) ?? defaultConstraint,
            _ => throw ExhaustiveMatch.Failed(this),
        };
}
#endregion

#region Expressions
public partial interface IExpressionNode
{
    /// <summary>
    /// Indicates that this node type should not actually be counted as an expression. (i.e. it
    /// shouldn't implement <see cref="IExpressionNode"/>).
    /// </summary>
    // TODO change inheritance so these are not expressions?
    public bool ShouldNotBeExpression()
        => this is INamespaceNameNode or IFunctionGroupNameNode or IMethodGroupNameNode
            or IInitializerGroupNameNode or ITypeNameExpressionNode;
}
#endregion
