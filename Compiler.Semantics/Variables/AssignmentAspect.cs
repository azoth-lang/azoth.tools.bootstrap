using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

internal static class AssignmentAspect
{
    public static FixedDictionary<IVariableBindingNode, int> ConcreteInvocableDefinition_VariableBindingsMap(IConcreteInvocableDefinitionNode node)
        => VariableBindings(node.Body);

    public static FixedDictionary<IVariableBindingNode, int> FieldDefinition_VariableBindingsMap(IFieldDefinitionNode node)
        => VariableBindings(node.Initializer);

    private static FixedDictionary<IVariableBindingNode, int> VariableBindings(IChildNode? node)
    {
        if (node is null) return FixedDictionary<IVariableBindingNode, int>.Empty;
        var localBindings = new List<IVariableBindingNode>();
        VariableBindings(node, localBindings);
        return localBindings.Enumerate().ToFixedDictionary();
    }

    private static void VariableBindings(ISemanticNode node, List<IVariableBindingNode> variableBindings)
    {
        switch (node)
        {
            case ITypeNameNode:
                // No need to recurse into type names
                return;
            case IVariableBindingNode variableBinding:
                variableBindings.Add(variableBinding);
                break;
        }
        foreach (var child in node.Children())
            VariableBindings(child, variableBindings);
    }

    public static BindingFlags<IVariableBindingNode> DataFlow_DefinitelyAssigned_Initial(IDataFlowNode node)
        => node.ControlFlowEntry().DefinitelyAssigned;

    public static BindingFlags<IVariableBindingNode> Entry_DefinitelyAssigned(IEntryNode node)
        // All local bindings are not yet assigned
        => BindingFlags.Create(node.VariableBindingsMap(), false);

    public static BindingFlags<IVariableBindingNode> VariableDeclarationStatement_DefinitelyAssigned(IVariableDeclarationStatementNode node)
    {
        var previous = node.DefinitelyAssignedPrevious();
        if (node.Initializer is null) return previous;
        return previous.Set(node, true);
    }

    public static BindingFlags<IVariableBindingNode> BindingPattern_DefinitelyAssigned(IBindingPatternNode node)
        // The lexical scope handles whether the binding is in scope for true vs false result. So
        // the binding is definitely assigned if it is in scope.
        => node.DefinitelyAssignedPrevious().Set(node, true);

    public static BindingFlags<IVariableBindingNode> ForeachExpression_DefinitelyAssigned(IForeachExpressionNode node)
        => node.DefinitelyAssignedPrevious().Set(node, true);

    public static BindingFlags<IVariableBindingNode> Exit_DefinitelyAssigned(IExitNode node)
        => node.DefinitelyAssignedPrevious();

    private static BindingFlags<IVariableBindingNode> DefinitelyAssignedPrevious(this IDataFlowNode node)
    {
        var previous = node.DataFlowPrevious;
        if (previous.IsEmpty) return node.ControlFlowEntry().DefinitelyAssigned;
        return previous.Select(d => d.DefinitelyAssigned).Aggregate((a, b) => a.Intersect(b));
    }
}
