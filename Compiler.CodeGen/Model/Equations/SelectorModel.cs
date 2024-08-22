using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(
    typeof(AllChildrenSelectorModel),
    typeof(NamedChildSelectorModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class SelectorModel
{
    public static SelectorModel Create(SelectorSyntax syntax)
        => syntax switch
        {
            AllChildrenSelectorSyntax syn => AllChildrenSelectorModel.Create(syn),
            ChildSelectorSyntax syn => new ChildSelectorModel(syn),
            ChildAtIndexSelectorSyntax syn => new ChildAtIndexSelectorModel(syn),
            ChildAtVariableSelectorSyntax syn => new ChildAtVariableSelectorModel(syn),
            ChildListSelectorSyntax syn => new ChildListSelectorModel(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public abstract SelectorSyntax Syntax { get; }
    /// <summary>
    /// Whether the selector should broadcast to all descendants of the selected node.
    /// </summary>
    public bool Broadcast { get; }
    public virtual bool IsAllDescendants => false;

    protected SelectorModel(bool broadcast)
    {
        Broadcast = broadcast;
    }

    /// <summary>
    /// The concrete nodes selected by this selector starting from the given node.
    /// </summary>
    public abstract IEnumerable<TreeNodeModel> SelectNodes(TreeNodeModel node);

    /// <summary>
    /// If this is a broadcasted selector, returns all concrete subtypes and concrete descendants
    /// of the given node. Otherwise, returns all concrete subtypes of the given node.
    /// </summary>
    protected IEnumerable<TreeNodeModel> BroadcastedToNodes(TreeNodeModel node)
        => BroadcastedToNodes([node]);

    /// <summary>
    /// If this is a broadcasted selector, returns all concrete subtypes and concrete descendants
    /// of the given nodes. Otherwise, returns all concrete subtypes of the given nodes.
    /// </summary>
    protected IEnumerable<TreeNodeModel> BroadcastedToNodes(IEnumerable<TreeNodeModel> nodes)
    {
        if (!Broadcast)
            return ConcreteSubtypes(nodes).Distinct();
        var descendants = ConcreteSubtypes(nodes).ToHashSet();
        var toProcess = new Queue<TreeNodeModel>(descendants);
        while (toProcess.TryDequeue(out var node))
            foreach (var child in ConcreteSubtypes(AllTreeChildNodes(node)))
                if (descendants.Add(child))
                    toProcess.Enqueue(child);
        return descendants;
    }

    /// <summary>
    /// All concrete subtypes of the given node (including the node itself if it is concrete).
    /// </summary>
    protected static IEnumerable<TreeNodeModel> ConcreteSubtypes(TreeNodeModel node)
        => node.DescendantNodes.Prepend(node).Where(n => !n.IsAbstract);

    /// <summary>
    /// All concrete subtypes of the given nodes (including the nodes themselves if they are concrete).
    /// </summary>
    protected static IEnumerable<TreeNodeModel> ConcreteSubtypes(IEnumerable<TreeNodeModel> nodes)
        => nodes.SelectMany(ConcreteSubtypes);

    /// <summary>
    /// The children nodes of the given node as determined by being child attributes of the node.
    /// </summary>
    /// <remarks>Works only on the given node and does not seek concrete subtypes.</remarks>
    protected static IEnumerable<TreeNodeModel> AllTreeChildNodes(TreeNodeModel node)
        // Only PropertyModel attributes are actual children
        => node.ActualAttributes.OfType<PropertyModel>().Select(a => a.Type.ReferencedNode()).WhereNotNull();

    public sealed override string ToString()
        => Broadcast ? $"{ToChildSelectorString()}.**" : ToChildSelectorString();

    protected abstract string ToChildSelectorString();
}
