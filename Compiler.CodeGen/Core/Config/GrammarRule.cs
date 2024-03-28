using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public sealed class GrammarRule
{
    public GrammarSymbol Defines { get; }
    public IFixedSet<GrammarSymbol> Parents { get; }
    public IFixedList<GrammarProperty> Properties { get; }

    public GrammarRule(
        GrammarSymbol defines,
        IEnumerable<GrammarSymbol> parents,
        IEnumerable<GrammarProperty> properties)
    {
        Defines = defines;
        Parents = parents.ToFixedSet();
        Properties = properties.ToFixedList();
        if (Properties.Select(p => p.Name).Distinct().Count() != Properties.Count)
            throw new ArgumentException($"Rule for {defines} contains duplicate property definitions");
    }

    public GrammarRule WithDefaultRootType(GrammarSymbol? rootType)
    {
        if (rootType is null
            || Parents.Any()
            || Defines == rootType) return this;
        return new GrammarRule(Defines, rootType.YieldValue(), Properties);
    }

    public override string ToString()
    {
        var parents = Parents.Count == 0 ? "" : ": " + string.Join(", ", Parents);
        var properties = Properties.Count == 0 ? "" : " = " + string.Join(", ", Properties);
        return $"{Defines}{parents}{properties};";
    }
}
