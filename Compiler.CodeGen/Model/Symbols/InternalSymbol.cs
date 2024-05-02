using System;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

public sealed class InternalSymbol : Symbol
{
    private readonly Grammar grammar;
    public string ShortName { get; }
    public override string FullName { get; }
    private readonly Lazy<Rule> referencedRule;
    public Rule ReferencedRule => referencedRule.Value;

    public InternalSymbol(Grammar grammar, string shortName)
    {
        this.grammar = grammar;
        ShortName = shortName;
        FullName = $"{grammar.Prefix}{shortName}{grammar.Suffix}";
        referencedRule = new(LookupReferencedRule);
        return;

        Rule LookupReferencedRule()
        {
            var rule = grammar.RuleFor(ShortName);
            if (rule is null)
                throw new FormatException($"Symbol '{ShortName}' must be quoted because it doesn't reference a rule.");
            return rule;
        }
    }

    #region Equality
    public override bool Equals(Symbol? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is InternalSymbol symbol
            && ShortName == symbol.ShortName
            // To avoid forcing lookup of the referenced rule, just compare the grammar reference
            && ReferenceEquals(grammar, symbol.grammar);
    }

    public override int GetHashCode()
        // To avoid forcing lookup of the referenced rule, just use the grammar reference
        => HashCode.Combine(ShortName, RuntimeHelpers.GetHashCode(grammar));
    #endregion

    public override int GetEquivalenceHashCode() => HashCode.Combine(ShortName);

    public override string ToString() => ShortName;
}
