using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class Symbol
{
    public static Symbol Void { get; } = new(null, SymbolNode.Void);

    public static Symbol? Create(Grammar? grammar, SymbolNode? syntax)
        => syntax is null ? null : new(grammar, syntax);

    public SymbolNode Syntax { get; }
    public string Name { get; }
    private readonly Lazy<Rule?> referencedRule;
    public Rule? ReferencedRule => referencedRule.Value;


    public Symbol(Grammar? grammar, SymbolNode syntax)
    {
        if (!syntax.IsQuoted && grammar is null)
            throw new ArgumentNullException(nameof(grammar), "Grammar must be provided for unquoted symbols.");

        Syntax = syntax;
        Name = Syntax.IsQuoted ? Syntax.Text : $"{grammar!.Prefix}{Syntax.Text}{grammar.Suffix}";
        referencedRule = new(() => grammar?.RuleFor(Syntax));
    }

    public override string ToString() => Syntax.ToString();
}
