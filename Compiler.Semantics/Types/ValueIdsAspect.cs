using System;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class ValueIdsAspect
{
    #region Definitions
    public static partial ValueIdScope InvocableDefinition_ValueIdScope(IInvocableDefinitionNode node)
        => new ValueIdScope();

    public static partial IPreviousValueId InvocableDefinition_Next_PreviousValueId(IInvocableDefinitionNode node)
        => new BeforeFirstValueId(node.ValueIdScope);
    #endregion

    #region Member Definitions
    public static partial ValueIdScope FieldDefinition_ValueIdScope(IFieldDefinitionNode node) => new ValueIdScope();

    public static partial ValueId FieldDefinition_BindingValueId(IFieldDefinitionNode node)
        => throw new NotImplementedException();
    #endregion

    #region Parameters
    public static partial ValueId Parameter_BindingValueId(IParameterNode node)
        => node.PreviousValueId().CreateNext();
    #endregion

    #region Statements
    public static partial ValueId VariableDeclarationStatement_BindingValueId(IVariableDeclarationStatementNode node)
        => node.PreviousValueId().CreateNext();
    #endregion

    #region Patterns
    public static partial ValueId BindingPattern_BindingValueId(IBindingPatternNode node)
        => node.PreviousValueId().CreateNext();
    #endregion

    #region Expressions
    // TODO have an alternate implementation that is easy to switch to that just assigned unique
    // value ids as they are requested. That would be much more efficient avoiding the need to
    // recompute value ids when they can't be cached because rewrites aren't finalized. Something
    // like a factory on the executable node. However, that would need special handling for the
    // framework to know that inheriting that through non-final nodes was still cacheable.
    public static partial ValueId AmbiguousExpression_ValueId(IAmbiguousExpressionNode node)
        => node.PreviousValueId().CreateNext();
    #endregion

    #region Control Flow Expressions
    public static partial ValueId ForeachExpression_BindingValueId(IForeachExpressionNode node)
        // Since value ids are in preorder, to makes some sense that the expression value id is
        // before the binding value id. However, this is also because it would be hard to change the
        // value id of the expression to depend on the binding value id, but it is easy to do this.
        => node.ValueId.CreateNext();
    #endregion
}
