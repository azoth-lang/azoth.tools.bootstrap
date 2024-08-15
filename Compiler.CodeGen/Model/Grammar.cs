using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Grammar
{
    public GrammarSyntax Syntax { get; }

    public string Namespace => Syntax.Namespace;
    public Symbol? DefaultParent { get; }
    public string Prefix => Syntax.Prefix;
    public string Suffix => Syntax.Suffix;
    public IFixedSet<string> UsingNamespaces => Syntax.UsingNamespaces;
    public IFixedList<Rule> Rules { get; }

    public Grammar(GrammarSyntax syntax)
    {
        Syntax = syntax;
        DefaultParent = Symbol.CreateFromSyntax(this, syntax.DefaultParent);
        Rules = syntax.Rules.Select(r => new Rule(this, r)).ToFixedList();
        rulesLookup = Rules.ToFixedDictionary(r => r.Defines.ShortName);
    }

    public Rule? RuleFor(string shortName)
        => rulesLookup.GetValueOrDefault(shortName);

    private readonly FixedDictionary<string, Rule> rulesLookup;

    public void ValidateTreeOrdering()
    {
        foreach (var rule in Rules.Where(r => r.IsTerminal))
        {
            var baseNonTerminalPropertyNames
                = rule.AncestorRules
                  .SelectMany(r => r.DeclaredProperties)
                  .Where(p => p.ReferencesRule).Select(p => p.Name);
            var nonTerminalPropertyNames = rule.DeclaredProperties.Where(p => p.ReferencesRule).Select(p => p.Name);
            var missingProperties = baseNonTerminalPropertyNames.Except(nonTerminalPropertyNames).ToList();
            if (missingProperties.Any())
                throw new ValidationException($"Rule for {rule.Defines} is missing inherited properties: {string.Join(", ", missingProperties)}. Can't determine order to visit children.");
        }
    }
}
