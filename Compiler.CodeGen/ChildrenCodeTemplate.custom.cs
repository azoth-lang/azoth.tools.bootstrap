using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Config;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen;

public partial class ChildrenCodeTemplate
{
    private readonly Grammar grammar;

    public ChildrenCodeTemplate(Grammar grammar)
    {
        this.grammar = grammar;
    }

    private string TypeName(GrammarSymbol symbol)
    {
        if (symbol.IsQuoted)
            return symbol.Text;

        // If it is a nonterminal, then transform the name
        if (grammar.Rules.Any(r => r.Nonterminal == symbol))
            return $"{grammar.Prefix}{symbol.Text}{grammar.Suffix}";

        return symbol.Text;
    }
}
