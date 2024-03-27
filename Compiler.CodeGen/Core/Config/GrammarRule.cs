using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public class GrammarRule
{
    public GrammarSymbol Nonterminal { get; }
    public IFixedList<GrammarSymbol> Parents { get; }

    public IFixedList<GrammarProperty> Properties { get; }

    public GrammarRule(
        GrammarSymbol nonterminal,
        IEnumerable<GrammarSymbol> parents,
        IEnumerable<GrammarProperty> properties)
    {
        Nonterminal = nonterminal;
        Parents = parents.ToFixedList();
        Properties = properties.ToFixedList();
    }
}
