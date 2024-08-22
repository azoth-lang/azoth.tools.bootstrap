using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(
    typeof(ChildSelectorModel),
    typeof(ChildAtIndexSelectorModel),
    typeof(ChildAtVariableSelectorModel),
    typeof(ChildListSelectorModel))]
public abstract class NamedChildSelectorModel : SelectorModel
{
    public string Child { get; }

    protected NamedChildSelectorModel(string child, bool broadcast)
        : base(broadcast)
    {
        Child = child;
    }

    public sealed override IEnumerable<TreeNodeModel> SelectNodes(TreeNodeModel node)
    {
        var attribute = node.ActualAttributes.Where(a => a.Name == Child).TrySingle();
        if (attribute is null)
            throw new FormatException($"Selector for child {Child} does not refer to a child attribute.");
        var referencedNode = attribute.Type.ReferencedNode();
        if (referencedNode is null)
            throw new FormatException($"Selector for child {Child} refers to a non-node attribute.");
        return BroadcastedToNodes(referencedNode);
    }
}
