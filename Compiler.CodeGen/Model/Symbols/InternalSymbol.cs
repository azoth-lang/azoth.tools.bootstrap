using System;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;

public sealed class InternalSymbol : Symbol
{
    private readonly TreeModel tree;
    public string ShortName { get; }
    public override string FullName { get; }
    private readonly Lazy<Rule> referencedRule;
    public Rule ReferencedRule => referencedRule.Value;

    public InternalSymbol(TreeModel tree, string shortName)
    {
        this.tree = tree;
        ShortName = shortName;
        FullName = $"{tree.SymbolPrefix}{shortName}{tree.SymbolSuffix}";
        referencedRule = new(LookupReferencedRule);
        return;

        Rule LookupReferencedRule()
        {
            var rule = tree.RuleFor(ShortName);
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
            // To avoid forcing lookup of the referenced rule, just compare the tree reference
            && ReferenceEquals(tree, symbol.tree);
    }

    public override int GetHashCode()
        // To avoid forcing lookup of the referenced rule, just use the tree reference
        => HashCode.Combine(ShortName, RuntimeHelpers.GetHashCode(tree));
    #endregion

    public override int GetEquivalenceHashCode() => HashCode.Combine(ShortName);

    public override string ToString() => ShortName;
}
