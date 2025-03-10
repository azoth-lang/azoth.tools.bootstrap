using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

/// <summary>
/// This aspect establishes a relationship between expressions and statements that follows the
/// control flow of the code.
/// </summary>
/// <remarks><para>A parent node is considered to come before the control flow of any children. For
/// example, the if-expression node comes before the condition of that if-expression which feeds into
/// the then- and else-expressions. Note that this is the case even for expressions where the
/// expression in some way executes after its children. For example, a function call is invoked
/// after the argument expressions. Nevertheless, the function invocation expression appears before
/// the argument expressions in this control flow graph. Another way of thinking of it is that an
/// expression node represents the expression as a whole. So if an expression is next in the control
/// flow graph, that indicates that whole expression is next.</para></remarks>
internal static partial class ControlFlowAspect
{
    #region Definitions
    public static partial IEntryNode ExecutableDefinition_Entry(IExecutableDefinitionNode node)
        => IEntryNode.Create();

    public static partial IExitNode ExecutableDefinition_Exit(IExecutableDefinitionNode node)
        => IExitNode.Create();

    public static partial ControlFlowSet InvocableDefinition_Entry_ControlFlowFollowing(IInvocableDefinitionNode node)
        => ControlFlowSet.CreateNormal(node.Body?.Statements.FirstOrDefault() ?? (IControlFlowNode)node.Exit);
    #endregion

    #region Control Flow
    public static partial void ControlFlow_Contribute_ControlFlow_ControlFlowPrevious(IControlFlowNode node, IControlFlowNode target, Dictionary<IControlFlowNode, ControlFlowKind> controlFlowPrevious)
    {
        if (node.ControlFlowNext.TryGetValue(target, out var kind))
            controlFlowPrevious.Add(node, kind);
    }

    public static partial ControlFlowSet Entry_ControlFlowNext(IEntryNode node)
        => node.ControlFlowFollowing();
    #endregion

    #region Statements
    public static partial ControlFlowSet ResultStatement_ControlFlowNext(IResultStatementNode node)
        => ControlFlowSet.CreateNormal(node.Expression);

    public static partial ControlFlowSet VariableDeclarationStatement_ControlFlowNext(IVariableDeclarationStatementNode node)
    {
        if (node.TempInitializer is not null)
            return ControlFlowSet.CreateNormal(node.Initializer);
        return node.ControlFlowFollowing();
    }

    public static partial ControlFlowSet ExpressionStatement_ControlFlowNext(IExpressionStatementNode node)
        => ControlFlowSet.CreateNormal(node.Expression);
    #endregion

    #region Patterns
    public static partial ControlFlowSet TypePattern_ControlFlowNext(ITypePatternNode node)
        => node.ControlFlowFollowing();

    public static partial ControlFlowSet BindingContextPattern_ControlFlowNext(IBindingContextPatternNode node)
        => ControlFlowSet.CreateNormal(node.Pattern);

    public static partial ControlFlowSet BindingPattern_ControlFlowNext(IBindingPatternNode node)
        => node.ControlFlowFollowing();

    public static partial ControlFlowSet OptionalPattern_ControlFlowNext(IOptionalPatternNode node)
        => ControlFlowSet.CreateNormal(node.Pattern);
    #endregion

    #region Expressions
    public static partial ControlFlowSet BlockExpression_ControlFlowNext(IBlockExpressionNode node)
    {
        if (!node.Statements.IsEmpty)
            return ControlFlowSet.CreateNormal(node.Statements[0]);
        return node.ControlFlowFollowing();
    }

    public static partial ControlFlowSet UnsafeExpression_ControlFlowNext(IUnsafeExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Expression);
    #endregion

    #region Unresolved Expressions
    public static partial ControlFlowSet UnresolvedMemberAccessExpression_ControlFlowNext(IUnresolvedMemberAccessExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);
    #endregion

    #region Instance Member Access Expressions
    public static partial ControlFlowSet FieldAccessExpression_ControlFlowNext(IFieldAccessExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);

    public static partial ControlFlowSet MethodAccessExpression_ControlFlowNext(IMethodAccessExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);
    #endregion

    #region Literal Expressions
    public static partial ControlFlowSet LiteralExpression_ControlFlowNext(ILiteralExpressionNode node)
        => node.ControlFlowFollowing();
    #endregion

    #region Operator Expressions
    public static partial ControlFlowSet AssignmentExpression_ControlFlowNext(IAssignmentExpressionNode node)
        => ControlFlowSet.CreateNormal(node.LeftOperand);

    public static partial ControlFlowSet RefAssignmentExpression_ControlFlowNext(IRefAssignmentExpressionNode node)
        => ControlFlowSet.CreateNormal(node.LeftOperand);

    public static partial ControlFlowSet BinaryOperatorExpression_ControlFlowNext(IBinaryOperatorExpressionNode node)
        => ControlFlowSet.CreateNormal(node.LeftOperand);

    public static partial ControlFlowSet UnaryOperatorExpression_ControlFlowNext(IUnaryOperatorExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Operand);

    public static partial ControlFlowSet ConversionExpression_ControlFlowNext(IConversionExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static partial ControlFlowSet ImplicitConversionExpression_ControlFlowNext(IImplicitConversionExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static partial ControlFlowSet OptionalConversionExpression_ControlFlowNext(IOptionalConversionExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static partial ControlFlowSet PatternMatchExpression_ControlFlowNext(IPatternMatchExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static partial ControlFlowSet RefExpression_ControlFlowNext(IRefExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static partial ControlFlowSet ImplicitDerefExpression_ControlFlowNext(IImplicitDerefExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);
    #endregion

    #region Control Flow Expressions
    public static partial ControlFlowSet IfExpression_ControlFlowNext(IIfExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Condition);

    public static partial ControlFlowSet LoopExpression_ControlFlowNext(ILoopExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Block);

    public static partial ControlFlowSet WhileExpression_ControlFlowNext(IWhileExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Condition);

    public static partial ControlFlowSet ForeachExpression_ControlFlowNext(IForeachExpressionNode node)
        => ControlFlowSet.CreateNormal(node.InExpression);

    public static partial ControlFlowSet BreakExpression_ControlFlowNext(IBreakExpressionNode node)
        // TODO this ought to be the ControlFlowNext of the loop being broken
        => node.ControlFlowFollowing();

    public static partial ControlFlowSet NextExpression_ControlFlowNext(INextExpressionNode node)
        // TODO this ought to be the next control flow node in a loop iteration of the loop being referred to
        => node.ControlFlowFollowing();

    public static partial ControlFlowSet ReturnExpression_ControlFlowNext(IReturnExpressionNode node)
        => ControlFlowSet.CreateNormal(node.TempValue is not null ? node.Value : node.ControlFlowExit());
    #endregion

    #region Invocation Expressions
    public static partial ControlFlowSet UnresolvedInvocationExpression_ControlFlowNext(IUnresolvedInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Expression);

    public static partial ControlFlowSet FunctionInvocationExpression_ControlFlowNext(IFunctionInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Function);

    public static partial ControlFlowSet MethodInvocationExpression_ControlFlowNext(IMethodInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Method);

    public static partial ControlFlowSet GetterInvocationExpression_ControlFlowNext(IGetterInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);

    public static partial ControlFlowSet SetterInvocationExpression_ControlFlowNext(ISetterInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);

    public static partial ControlFlowSet FunctionReferenceInvocationExpression_ControlFlowNext(IFunctionReferenceInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Expression);

    public static partial ControlFlowSet InitializerInvocationExpression_ControlFlowNext(IInitializerInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Initializer);

    public static partial ControlFlowSet NonInvocableInvocationExpression_ControlFlowNext(INonInvocableInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Expression);
    #endregion

    #region Name Expressions
    public static partial ControlFlowSet NameExpression_ControlFlowNext(INameExpressionNode node)
        => node.ControlFlowFollowing();
    #endregion

    #region Unresolved Name Expressions
    public static partial ControlFlowSet UnresolvedQualifiedNameExpression_ControlFlowNext(IUnresolvedQualifiedNameExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);
    #endregion

    #region Capability Expressions
    public static partial ControlFlowSet RecoveryExpression_ControlFlowNext(IRecoveryExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static partial ControlFlowSet ImplicitTempMoveExpression_ControlFlowNext(IImplicitTempMoveExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static partial ControlFlowSet PrepareToReturnExpression_ControlFlowNext(IPrepareToReturnExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Value);
    #endregion

    #region Async Expressions
    public static partial ControlFlowSet AsyncBlockExpression_ControlFlowNext(IAsyncBlockExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Block);

    public static partial ControlFlowSet AsyncStartExpression_ControlFlowNext(IAsyncStartExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Expression);

    public static partial ControlFlowSet AwaitExpression_ControlFlowNext(IAwaitExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Expression);
    #endregion
}
