using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
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

    public static BindingFlags<ILocalBindingNode> Entry_DefinitelyAssigned(IEntryNode node)
        // All local bindings are not yet assigned
        => BindingFlags.Create(node.LocalBindingsMap(), false);

    public static BindingFlags<ILocalBindingNode> Exit_DefinitelyAssigned(IExitNode node)
        // TODO implement proper definite assignment analysis
        => BindingFlags.Create(FixedDictionary<ILocalBindingNode, int>.Empty, false);
    //=> node.DefinitelyAssignedPrevious();

    private static BindingFlags<ILocalBindingNode> DefinitelyAssignedPrevious(this IDataFlowNode node)
        => node.DataFlowPrevious(n => n.DefinitelyAssigned, (a, b) => a.Intersect(b));
}
