using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class GrammarNode
{
    public string Namespace { get; }
    public SymbolNode? DefaultParent { get; }
    public string Prefix { get; }
    public string Suffix { get; }
    public string ListType { get; }
    public string SetType { get; }
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedList<RuleNode> Rules { get; }

    public GrammarNode(
        string @namespace,
        SymbolNode? defaultParent,
        string prefix,
        string suffix,
        string listType,
        string setType,
        IEnumerable<string> usingNamespaces,
        IEnumerable<RuleNode> rules)
    {
        Namespace = @namespace;
        DefaultParent = defaultParent;
        Prefix = prefix;
        Suffix = suffix;
        ListType = listType;
        SetType = setType;
        UsingNamespaces = usingNamespaces.ToFixedSet();
        Rules = rules.ToFixedList();
        if (Rules.Select(r => r.Defines).Distinct().Count() != Rules.Count)
            throw new ValidationException("Rule names must be unique");
    }
}
