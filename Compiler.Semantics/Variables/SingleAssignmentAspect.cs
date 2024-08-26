using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

/// <summary>
/// This uses a data flow analysis to determine if a `let` binding may be assigned more than once.
/// The analysis is based off the variables that are definitely unassigned.
/// </summary>
internal static class SingleAssignmentAspect
{
    public static BindingFlags<IVariableBindingNode> DataFlow_DefinitelyUnassigned_Initial(IDataFlowNode node)
        // Since the merge operation is intersection, the initial state must be that all bindings
        // are definitely unassigned. That is the same as the entry state.
        => node.ControlFlowEntry().DefinitelyUnassigned;

    public static BindingFlags<IVariableBindingNode> Entry_DefinitelyUnassigned(IEntryNode node)
        // All local bindings are definitely unassigned
        => BindingFlags.Create(node.VariableBindingsMap(), true);


    public static BindingFlags<IVariableBindingNode> VariableDeclarationStatement_DefinitelyUnassigned(IVariableDeclarationStatementNode node)
    {
        var previous = node.DefinitelyUnassignedPrevious();
        if (node.TempInitializer is null) return previous;
        // TODO this is technically marking it as assigned inside the initializer too. (Of course it isn't in scope there)
        return node.DefinitelyUnassignedPrevious().Set(node, false);
    }

    public static BindingFlags<IVariableBindingNode> BindingPattern_DefinitelyUnassigned(IBindingPatternNode node)
        => node.DefinitelyUnassignedPrevious().Set(node, false);

    public static BindingFlags<IVariableBindingNode> ForeachExpression_DefinitelyUnassigned(IForeachExpressionNode node)
        => node.DefinitelyUnassignedPrevious().Set(node, false);

    public static BindingFlags<IVariableBindingNode> AssignmentExpression_DefinitelyUnassigned(IAssignmentExpressionNode node)
    {
        var previous = node.DefinitelyUnassignedPrevious();
        if (node.IntermediateLeftOperand is IVariableNameExpressionNode { ReferencedDefinition: IVariableBindingNode variableBinding })
            return previous.Set(variableBinding, false);
        return previous;
    }

    public static BindingFlags<IVariableBindingNode> Exit_DefinitelyUnassigned(IExitNode node)
        => node.DefinitelyUnassignedPrevious();

    private static BindingFlags<IVariableBindingNode> DefinitelyUnassignedPrevious(this IDataFlowNode node)
    {
        var previous = node.DataFlowPrevious;
        return DefinitelyUnassignedPrevious(node, previous);
    }

    private static BindingFlags<IVariableBindingNode> DefinitelyUnassignedPrevious(
        IControlFlowNode node,
        IFixedSet<IDataFlowNode> previous)
    {
        if (previous.IsEmpty) return node.ControlFlowEntry().DefinitelyUnassigned;
        return previous.Select(d => d.DefinitelyUnassigned).Aggregate((a, b) => a.Intersect(b));
    }

    public static void VariableNameExpression_ContributeDiagnostics(IVariableNameExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node is not
            {
                Parent: IAssignmentExpressionNode assignment,
                ReferencedDefinition: IVariableBindingNode { IsMutableBinding: false } variableBinding
            }
            || assignment.TempLeftOperand != node)
            return;

        var definitelyUnassigned = DefinitelyUnassignedPrevious(node, node.DataFlowPrevious);
        if (!definitelyUnassigned[variableBinding])
            diagnostics.Add(OtherSemanticError.MayAlreadyBeAssigned(node.File, node.Syntax.Span, node.Name));
    }
}
