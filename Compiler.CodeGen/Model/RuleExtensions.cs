using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

internal static class RuleExtensions
{
    /// <summary>
    /// Eliminate rules that are redundant because they are a parent of another rule.
    /// </summary>
    public static IEnumerable<Rule> EliminateRedundantRules(this IEnumerable<Rule> rules)
    {
        var ruleSet = rules.ToFixedSet();
        var parentRules = ruleSet.SelectMany(r => r.ParentRules).ToFixedSet();
        return ruleSet.Except(parentRules);
    }
}