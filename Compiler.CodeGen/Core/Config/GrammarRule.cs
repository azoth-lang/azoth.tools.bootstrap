using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public sealed class GrammarRule
{
    public GrammarSymbol Nonterminal { get; }
    public IFixedSet<GrammarSymbol> Parents { get; }
    public IFixedList<GrammarProperty> Properties { get; }

    public GrammarRule(
        GrammarSymbol nonterminal,
        IEnumerable<GrammarSymbol> parents,
        IEnumerable<GrammarProperty> properties)
    {
        Nonterminal = nonterminal;
        Parents = parents.ToFixedSet();
        Properties = properties.ToFixedList();
        if (Properties.Select(p => p.Name).Distinct().Count() != Properties.Count)
            throw new ArgumentException($"Rule for {nonterminal} contains duplicate property definitions");
    }

    public GrammarRule WithDefaultRootType(GrammarSymbol? baseType)
    {
        if (baseType is null
            || Parents.Any()
            || Nonterminal == baseType) return this;
        return new GrammarRule(Nonterminal, baseType.YieldValue(), Properties);
    }
}
