using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Build
{
    public static IEnumerable<string> OrderedNamespaces(Grammar grammar, params string[] additionalNamespaces)
        => grammar.UsingNamespaces.Concat(additionalNamespaces).Distinct().OrderBy(v => v, NamespaceComparer.Instance);

    public static IEnumerable<Property> BaseProperties(Rule rule, Property property)
        => rule.InheritedPropertiesNamed(property)
        .Concat(rule.InheritedPropertiesWithoutMostSpecificImplementationNamed(property))
        .Where(p => p.IsDeclared);
}
