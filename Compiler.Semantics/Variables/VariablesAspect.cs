using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

internal static class VariablesAspect
{
    public static FixedDictionary<IVariableBindingNode, int> ConcreteInvocableDefinition_VariableBindingsMap(IConcreteInvocableDefinitionNode node)
        => VariableBindings(node.Body);

    public static FixedDictionary<IVariableBindingNode, int> FieldDefinition_VariableBindingsMap(IFieldDefinitionNode node)
        => VariableBindings(node.TempInitializer);

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
        foreach (var child in node.Children()) VariableBindings(child, variableBindings);
    }
}
