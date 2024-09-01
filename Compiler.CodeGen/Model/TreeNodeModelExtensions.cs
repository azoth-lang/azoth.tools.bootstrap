using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

internal static class TreeNodeModelExtensions
{
    /// <summary>
    /// Eliminate nodes that are redundant because they are a parent of another rule.
    /// </summary>
    public static IEnumerable<TreeNodeModel> EliminateRedundantRules(this IEnumerable<TreeNodeModel> nodes)
    {
        var nodeSet = nodes.ToFixedSet();
        var supertypeRules = nodeSet.SelectMany(r => r.SupertypeNodes).ToFixedSet();
        return nodeSet.Except(supertypeRules);
    }

    /// <summary>
    /// All concrete subtypes of the given node (including the node itself if it is concrete).
    /// </summary>
    public static IEnumerable<TreeNodeModel> ConcreteSubtypes(this TreeNodeModel node)
        => node.DescendantNodes.Prepend(node).Where(n => !n.IsAbstract);

    /// <summary>
    /// All concrete subtypes of the given nodes (including the nodes themselves if they are concrete).
    /// </summary>
    public static IEnumerable<TreeNodeModel> ConcreteSubtypes(this IEnumerable<TreeNodeModel> nodes)
        => nodes.SelectMany(ConcreteSubtypes);
}
