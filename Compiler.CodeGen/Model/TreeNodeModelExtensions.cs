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
}
