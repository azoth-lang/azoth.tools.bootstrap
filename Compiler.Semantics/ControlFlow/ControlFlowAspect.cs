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
internal sealed class ControlFlowAspect
{
    public static ControlFlowSet ConcreteInvocableDefinition_InheritedControlFlowFollowing_Entry(IConcreteInvocableDefinitionNode node)
        => ControlFlowSet.CreateNormal(node.Body?.Statements.FirstOrDefault() ?? (IControlFlowNode)node.Exit);

    public static ControlFlowSet Entry_ControlFlowNext(IEntryNode node)
        => node.ControlFlowFollowing();

    public static ControlFlowSet ExpressionStatement_ControlFlowNext(IExpressionStatementNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateExpression);

    public static ControlFlowSet VariableDeclarationStatement_ControlFlowNext(
        IVariableDeclarationStatementNode node)
    {
        if (node.Initializer is not null)
            return ControlFlowSet.CreateNormal(node.IntermediateInitializer);
        return node.ControlFlowFollowing();
    }

    public static ControlFlowSet ResultStatement_ControlFlowNext(IResultStatementNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateExpression);

    public static ControlFlowSet Expression_ControlFlowNext(IExpressionNode node)
        => node.ControlFlowFollowing();

    public static ControlFlowSet FunctionInvocationExpression_ControlFlowNext(
        IFunctionInvocationExpressionNode node)
        // TODO this shouldn't just be a function group, but instead a function name.
        => ControlFlowSet.CreateNormal(node.FunctionGroup);

    public static ControlFlowSet FunctionReferenceInvocation_ControlFlowNext(
        IFunctionReferenceInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Expression);

    public static ControlFlowSet MethodInvocationExpression_ControlFlowNext(
        IMethodInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.MethodGroup);

    public static ControlFlowSet FieldAccessExpression_ControlFlowNext(IFieldAccessExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);

    public static ControlFlowSet GetterInvocationExpression_ControlFlowNext(IGetterInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);

    public static ControlFlowSet AssignmentExpression_ControlFlowNext(IAssignmentExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateLeftOperand);

    public static ControlFlowSet SetterInvocationExpression_ControlFlowNext(ISetterInvocationExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Context);

    public static ControlFlowSet UnsafeExpression_ControlFlowNext(IUnsafeExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateExpression);

    public static ControlFlowSet MethodGroupName_ControlFlowNext(
        IMethodGroupNameNode node)
        => ControlFlowSet.CreateNormal(node.Context);

    public static ControlFlowSet NewObjectExpression_ControlFlowNext(
        INewObjectExpressionNode node)
    {
        if (!node.Arguments.IsEmpty)
            return ControlFlowSet.CreateNormal(node.IntermediateArguments[0]);
        return node.ControlFlowFollowing();
    }

    public static ControlFlowSet IfExpression_ControlFlowNext(
        IIfExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateCondition);

    public static ControlFlowSet BinaryOperatorExpression_ControlFlowNext(
        IBinaryOperatorExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateLeftOperand);

    public static ControlFlowSet ImplicitConversionExpression_ControlFlowNext(IImplicitConversionExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static ControlFlowSet ConversionExpression_ControlFlowNext(IConversionExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateReferent);

    public static ControlFlowSet ReturnExpression_ControlFlowNext(
        IReturnExpressionNode node)
    {
        if (node.Value is not null)
            return ControlFlowSet.CreateNormal(node.IntermediateValue);
        return ControlFlowSet.CreateNormal(node.ControlFlowExit());
    }

    public static ControlFlowSet WhileExpression_ControlFlowNext(IWhileExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateCondition);

    public static ControlFlowSet LoopExpression_ControlFlowNext(ILoopExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Block);

    public static ControlFlowSet ForeachExpression_ControlFlowNext(IForeachExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateInExpression);

    public static ControlFlowSet UnaryOperatorExpression_ComputeControlFlowNext(IUnaryOperatorExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateOperand);

    public static ControlFlowSet BlockExpression_ControlFlowNext(IBlockExpressionNode node)
    {
        if (!node.Statements.IsEmpty)
            return ControlFlowSet.CreateNormal(node.Statements[0]);
        return node.ControlFlowFollowing();
    }

    public static ControlFlowSet FreezeExpression_ControlFlowNext(IFreezeExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static ControlFlowSet MoveExpression_ControlFlowNext(IMoveExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static ControlFlowSet ImplicitTempMoveExpression_ControlFlowNext(IImplicitTempMoveExpressionNode node)
        => ControlFlowSet.CreateNormal(node.Referent);

    public static ControlFlowSet IdExpression_ControlFlowNext(IIdExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateReferent);

    public static ControlFlowSet AwaitExpression_ControlFlowNext(IAwaitExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateExpression);

    public static ControlFlowSet PatternMatchExpression_ControlFlowNext(IPatternMatchExpressionNode node)
        => ControlFlowSet.CreateNormal(node.IntermediateReferent);

    public static ControlFlowSet BindingPattern_ControlFlowNext(IBindingPatternNode node)
        => node.ControlFlowFollowing();

    public static ControlFlowSet OptionalPattern_ControlFlowNext(IOptionalPatternNode node)
        => ControlFlowSet.CreateNormal(node.Pattern);

    public static ControlFlowSet BindingContextPattern_ControlFlowNext(IBindingContextPatternNode node)
        => ControlFlowSet.CreateNormal(node.Pattern);

    public static void ControlFlow_ContributeControlFlowPrevious(
        IControlFlowNode node,
        IControlFlowNode target,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        if (node.ControlFlowNext.TryGetValue(target, out var kind))
            previous.Add(node, kind);
    }
}
