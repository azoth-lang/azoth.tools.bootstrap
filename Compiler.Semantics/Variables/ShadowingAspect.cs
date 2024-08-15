using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

internal static class ShadowingAspect
{
    public static void VariableBinding_ContributeDiagnostics(IVariableBindingNode node, DiagnosticsBuilder diagnostics)
    {
        if (node.ContainingLexicalScope.Lookup(node.Name).TrySingle() is INamedBindingNode shadowedDeclaration)
        {
            if (shadowedDeclaration.IsMutableBinding)
                diagnostics.Add(OtherSemanticError.CantRebindMutableBinding(node.File, node.Syntax.NameSpan));
            else if (node.IsMutableBinding)
                diagnostics.Add(OtherSemanticError.CantRebindAsMutableBinding(node.File, node.Syntax.NameSpan));
        }
    }

    public static void VariableNameExpression_ContributeDiagnostics(IVariableNameExpressionNode node, DiagnosticsBuilder diagnostics)
    {
        // If it is a mutable binding, then no shadowing is possible
        if (node.ReferencedDefinition.IsMutableBinding)
            return;

        // TODO this causes an error for every use of the variable after it was shadowed. That is not ideal
        // One way to solve this would be to have the uses contribute to a collection of uses on the
        // shadowed by binding. Then, report a diagnostic for the first use of the shadowed variable
        foreach (var shadowedBy in node.ReferencedBindingShadowedBy())
            diagnostics.Add(OtherSemanticError.CantShadow(shadowedBy.File, shadowedBy.Syntax.NameSpan, node.Syntax.Span));
    }

    private static IEnumerable<ILocalBindingNode> ReferencedBindingShadowedBy(this IVariableNameExpressionNode node)
    {
        var localBinding = node.ReferencedDefinition;
        // TODO this is a little odd because ILocalBindingNode is not an IDataFlowNode, so use ISemanticNode
        // Mark the binding as visited to avoid searching past it
        var visitedNodes = new HashSet<ISemanticNode>() { localBinding };
        var nodeStack = new Stack<IDataFlowNode>();
        nodeStack.PushRange(node.DataFlowPrevious);

        while (nodeStack.TryPop(out var current))
        {
            visitedNodes.Add(current);
            if (current is ILocalBindingNode otherBinding
                && otherBinding.Name == localBinding.Name
                && otherBinding.ContainingLexicalScope.Lookup(otherBinding.Name).TrySingle() is INamedBindingNode shadowedDeclaration
                && shadowedDeclaration == localBinding)
            {
                yield return otherBinding;
            }

            nodeStack.PushRange(current.DataFlowPrevious.Except(visitedNodes.OfType<IDataFlowNode>()));
        }
    }
}
