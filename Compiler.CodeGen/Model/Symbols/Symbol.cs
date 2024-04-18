using System;
using System.Collections.Generic;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class Symbol : IEquatable<Symbol>
{
    public static IEqualityComparer<Symbol> NameComparer { get; }
        = EqualityComparer<Symbol>.Create((x, y) => x?.Name == y?.Name, x => HashCode.Combine(x.Name));

    private readonly Grammar? grammar;
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

        this.grammar = grammar;
        Syntax = syntax;
        Name = Syntax.IsQuoted ? Syntax.Text : $"{grammar!.Prefix}{Syntax.Text}{grammar.Suffix}";
        referencedRule = new(LookupReferencedRule);
        return;
        Rule? LookupReferencedRule()
        {
            var rule = grammar?.RuleFor(Syntax);
            if (rule is null && !Syntax.IsQuoted)
                throw new FormatException($"Symbol '{Syntax}' must be quoted because it doesn't reference a rule.");
            return rule;
        }
    }

    #region Equality
    public bool Equals(Symbol? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Syntax == other.Syntax
               && (Syntax.IsQuoted || ReferenceEquals(grammar, other.grammar));
    }

    public override bool Equals(object? obj) => obj is Symbol other && Equals(other);

    public override int GetHashCode()
    {
        if (Syntax.IsQuoted)
            return HashCode.Combine(Syntax);
        return HashCode.Combine(Syntax, grammar);
    }

    public static bool operator ==(Symbol? left, Symbol? right) => Equals(left, right);

    public static bool operator !=(Symbol? left, Symbol? right) => !Equals(left, right);
    #endregion

    public override string ToString() => Syntax.ToString();
}
