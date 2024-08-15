using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class GrammarSyntax
{
    public string Namespace { get; }
    public SymbolSyntax? DefaultParent { get; }
    public string SymbolPrefix { get; }
    public string SymbolSuffix { get; }
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedList<RuleSyntax> Rules { get; }

    public GrammarSyntax(
        string @namespace,
        SymbolSyntax? defaultParent,
        string symbolPrefix,
        string symbolSuffix,
        IEnumerable<string> usingNamespaces,
        IEnumerable<RuleSyntax> rules)
    {
        Namespace = @namespace;
        DefaultParent = defaultParent;
        SymbolPrefix = symbolPrefix;
        SymbolSuffix = symbolSuffix;
        UsingNamespaces = usingNamespaces.ToFixedSet();
        Rules = rules.ToFixedList();
        if (Rules.Select(r => r.Defines).Distinct().Count() != Rules.Count)
            throw new ValidationException("Rule names must be unique");
    }
}
