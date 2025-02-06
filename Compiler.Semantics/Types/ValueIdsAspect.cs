using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class ValueIdsAspect
{
    #region Definitions
    public static partial ValueIdScope ExecutableDefinition_ValueIdScope(IExecutableDefinitionNode node)
        => new ValueIdScope();
    #endregion

    #region Type Definitions
    public static partial ValueIdScope TypeDefinition_ValueIdScope(ITypeDefinitionNode node)
        => new ValueIdScope();
    #endregion

    #region Member Definitions
    public static partial ValueId FieldDefinition_BindingValueId(IFieldDefinitionNode node)
        => throw new NotImplementedException();
    #endregion

    #region Parameters
    public static partial ValueId Parameter_BindingValueId(IParameterNode node)
        => node.ValueIdScope.CreateNext();
    #endregion

    #region Statements
    public static partial ValueId VariableDeclarationStatement_BindingValueId(IVariableDeclarationStatementNode node)
        => node.ValueIdScope.CreateNext();
    #endregion

    #region Patterns
    public static partial ValueId BindingPattern_BindingValueId(IBindingPatternNode node)
        => node.ValueIdScope.CreateNext();
    #endregion

    #region Expressions
    public static partial ValueId Expression_ValueId(IExpressionNode node)
        => node.ValueIdScope.CreateNext();
    #endregion

    #region Control Flow Expressions
    public static partial ValueId ForeachExpression_ImplicitRecoveryValueId(IForeachExpressionNode node)
        => node.ValueIdScope.CreateNext();

    public static partial ValueId ForeachExpression_IteratorValueId(IForeachExpressionNode node)
        => node.ValueIdScope.CreateNext();

    public static partial ValueId ForeachExpression_BindingValueId(IForeachExpressionNode node)
        => node.ValueIdScope.CreateNext();
    #endregion
}
