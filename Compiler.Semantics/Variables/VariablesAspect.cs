using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

internal static partial class VariablesAspect
{
    public static partial FixedDictionary<IVariableBindingNode, int> InvocableDefinition_VariableBindingsMap(IInvocableDefinitionNode node)
        => VariableBindings(node.Body);

    public static partial FixedDictionary<IVariableBindingNode, int> FieldDefinition_VariableBindingsMap(IFieldDefinitionNode node)
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
