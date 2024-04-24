using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Build
{
    public static IEnumerable<string> OrderedNamespaces(Grammar grammar, params string[] additionalNamespaces)
        => grammar.UsingNamespaces.Concat(additionalNamespaces).Distinct().OrderBy(v => v, NamespaceComparer.Instance);

    public static IEnumerable<string> OrderedNamespaces(Pass pass, params string[] additionalNamespaces)
    {
        var fromNamespaces = pass.FromLanguage?.Grammar.UsingNamespaces ?? FixedSet.Empty<string>();
        var toNamespaces = pass.ToLanguage?.Grammar.UsingNamespaces ?? FixedSet.Empty<string>();

        return pass.UsingNamespaces
                   .Concat(fromNamespaces)
                   .Concat(toNamespaces)
                   .Concat(additionalNamespaces)
                   .Distinct().OrderBy(v => v, NamespaceComparer.Instance);
    }

    public static IEnumerable<Rule> SimpleCreateRules(Pass pass)
    {
        if (pass.FromLanguage == pass.ToLanguage || pass.ToLanguage is null)
            return Enumerable.Empty<Rule>();
        return pass.ToLanguage.Grammar.Rules.Where(r => r is { IsTerminal: true, DescendantsModified: true });
    }
}
