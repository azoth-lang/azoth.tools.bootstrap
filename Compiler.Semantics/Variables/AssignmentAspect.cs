using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
internal static class AssignmentAspect
{
    public static FixedDictionary<ILocalBindingNode, int> ConcreteInvocableDefinition_LocalBindingsMap(IConcreteInvocableDefinitionNode node)
        => LocalBindings(node.Body);

    public static FixedDictionary<ILocalBindingNode, int> FieldDefinition_LocalBindingsMap(IFieldDefinitionNode node)
        => LocalBindings(node.Initializer);

    private static FixedDictionary<ILocalBindingNode, int> LocalBindings(IChildNode? node)
    {
        if (node is null) return FixedDictionary<ILocalBindingNode, int>.Empty;
        var localBindings = new List<ILocalBindingNode>();
        LocalBindings(node, localBindings);
        return localBindings.Enumerate().ToFixedDictionary();
    }

    private static void LocalBindings(ISemanticNode node, List<ILocalBindingNode> localBindings)
    {
        switch (node)
        {
            case ITypeNameNode:
                // No need to recurse into type names
                return;
            case ILocalBindingNode localBinding:
                localBindings.Add(localBinding);
                break;
        }
        foreach (var child in node.Children())
            LocalBindings(child, localBindings);
    }
}
