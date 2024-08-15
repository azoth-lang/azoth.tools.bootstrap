using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class GrammarSyntax
{
    public string Namespace { get; }
    public SymbolSyntax? DefaultParent { get; }
    public string Prefix { get; }
    public string Suffix { get; }
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedList<RuleSyntax> Rules { get; }

    public GrammarSyntax(
        string @namespace,
        SymbolSyntax? defaultParent,
        string prefix,
        string suffix,
        IEnumerable<string> usingNamespaces,
        IEnumerable<RuleSyntax> rules)
    {
        Namespace = @namespace;
        DefaultParent = defaultParent;
        Prefix = prefix;
        Suffix = suffix;
        UsingNamespaces = usingNamespaces.ToFixedSet();
        Rules = rules.ToFixedList();
        if (Rules.Select(r => r.Defines).Distinct().Count() != Rules.Count)
            throw new ValidationException("Rule names must be unique");
    }
}
