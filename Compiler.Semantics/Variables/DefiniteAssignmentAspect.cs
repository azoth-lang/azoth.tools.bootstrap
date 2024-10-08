using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

internal static partial class DefiniteAssignmentAspect
{
    public static partial BindingFlags<IVariableBindingNode> DataFlow_DefinitelyAssigned_Initial(IDataFlowNode node)
        // Since the merge operation is intersection, the initial state must be that all bindings are "assigned".
        => BindingFlags.Create(node.ControlFlowEntry().VariableBindingsMap(), true);

    public static partial BindingFlags<IVariableBindingNode> Entry_DefinitelyAssigned(IEntryNode node)
        // All local bindings are not yet assigned
        => BindingFlags.Create(node.VariableBindingsMap(), false);

    public static partial BindingFlags<IVariableBindingNode> VariableDeclarationStatement_DefinitelyAssigned(IVariableDeclarationStatementNode node)
    {
        var previous = node.DefinitelyAssignedPrevious();
        if (node.TempInitializer is null) return previous;
        // TODO this is technically marking it as assigned inside the initializer too. (Of course it isn't in scope there)
        return previous.Set(node, true);
    }

    public static partial BindingFlags<IVariableBindingNode> BindingPattern_DefinitelyAssigned(IBindingPatternNode node)
        // The lexical scope handles whether the binding is in scope for true vs false result. So
        // the binding is definitely assigned if it is in scope.
        => node.DefinitelyAssignedPrevious().Set(node, true);

    public static partial BindingFlags<IVariableBindingNode> ForeachExpression_DefinitelyAssigned(IForeachExpressionNode node)
        => node.DefinitelyAssignedPrevious().Set(node, true);

    public static partial BindingFlags<IVariableBindingNode> AssignmentExpression_DefinitelyAssigned(IAssignmentExpressionNode node)
    {
        var previous = node.DefinitelyAssignedPrevious();
        if (node.LeftOperand is IVariableNameExpressionNode { ReferencedDefinition: IVariableBindingNode variableBinding })
            return previous.Set(variableBinding, true);
        return previous;
    }

    public static partial BindingFlags<IVariableBindingNode> Exit_DefinitelyAssigned(IExitNode node)
        => node.DefinitelyAssignedPrevious();

    private static BindingFlags<IVariableBindingNode> DefinitelyAssignedPrevious(this IDataFlowNode node)
    {
        var previous = node.DataFlowPrevious;
        return DefinitelyAssignedPrevious(node, previous);
    }

    private static BindingFlags<IVariableBindingNode> DefinitelyAssignedPrevious(IControlFlowNode node, IFixedSet<IDataFlowNode> previous)
    {
        if (previous.IsEmpty) return node.ControlFlowEntry().DefinitelyAssigned;
        return previous.Select(d => d.DefinitelyAssigned).Aggregate((a, b) => a.Intersect(b));
    }

    public static partial void VariableNameExpression_Contribute_Diagnostics(IVariableNameExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node is not { ReferencedDefinition: IVariableBindingNode variableBinding })
            return;

        var definitelyAssigned = DefinitelyAssignedPrevious(node, node.DataFlowPrevious);
        if (!definitelyAssigned[variableBinding])
            diagnostics.Add(OtherSemanticError.VariableMayNotHaveBeenAssigned(node.File, node.Syntax.Span, node.Name));
    }
}
