using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

internal static class LanguageParser
{
    public static Language ParseLanguage(string inputPath, string input)
    {
        var lines = Parsing.ParseLines(input).ToFixedList();

        var name = Parsing.GetConfig(lines, "name") ?? throw new FormatException("Language name is required");
        var usingNamespaces = Parsing.ParseUsingNamespaces(lines).ToFixedList();

        var extendsLanguageName = Parsing.GetConfig(lines, "extends");
        if (extendsLanguageName is not null)
        {
            var extendedLanguagePath = Path.Combine(Path.GetDirectoryName(inputPath)!, Path.ChangeExtension(extendsLanguageName, "lang"));
            Console.WriteLine($"Extending : {extendedLanguagePath}");
            var extendedLanguageInputFile = File.ReadAllText(extendedLanguagePath)
                            ?? throw new InvalidOperationException($"null from reading file {extendedLanguagePath}");
            var extendsLanguage = ParseLanguage(extendedLanguagePath, extendedLanguageInputFile);

            var extends = extendsLanguage.Grammar;
            var rules = ParseRuleExtensions(extendsLanguage, lines, extends.RootType).ToFixedList();
            var grammar = new Grammar(extends.Namespace, extends.RootType, extends.Prefix, extends.Suffix, extends.ListType, usingNamespaces, rules);
            return new Language(name, grammar, extendsLanguage);
        }
        else
        {
            var ns = Parsing.GetConfig(lines, "namespace");
            var rootType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "root"));
            var prefix = Parsing.GetConfig(lines, "prefix") ?? "";
            var suffix = Parsing.GetConfig(lines, "suffix") ?? "";
            var listType = Parsing.GetConfig(lines, "list") ?? "List";
            var rules = Parsing.ParseRules(lines, rootType).ToFixedList();
            var grammar = new Grammar(ns, rootType, prefix, suffix, listType, usingNamespaces, rules);
            return new Language(name, grammar, extends: null);
        }
    }

    private static IEnumerable<GrammarRule> ParseRuleExtensions(Language extends, IEnumerable<string> lines, GrammarSymbol? rootType)
        => ParseRuleExtensions(extends, lines).Select(r => r.WithDefaultRootType(rootType));

    public static IEnumerable<GrammarRule> ParseRuleExtensions(Language extends, IEnumerable<string> lines)
    {
        var extendingRules = extends.Grammar.Rules.ToDictionary(r => r.Defines);
        var statements = Parsing.ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
        {
            var rule = ParseRuleExtension(statement, extendingRules);
            if (rule is not null)
                yield return rule;
        }

        // Emit any unchanged rules
        foreach (var rule in extendingRules.Values)
            yield return rule;
    }

    private static GrammarRule? ParseRuleExtension(
        string statement,
        Dictionary<GrammarSymbol, GrammarRule> extendingRules)
        => statement[0] switch
        {
            '+' => ParseRuleAddition(statement[1..]),
            '*' => ParseRuleModification(statement[1..], extendingRules),
            '-' => ParseRuleRemoval(statement[1..], extendingRules),
            _ => throw new FormatException($"Invalid rule extension statement: '{statement}'")
        };

    private static GrammarRule ParseRuleAddition(string statement)
        // Nothing to do but parse the new rule
        => Parsing.ParseRule(statement);

    private static GrammarRule ParseRuleModification(
        string statement,
        IDictionary<GrammarSymbol, GrammarRule> extendingRules)
    {
        var (declaration, definition) = Parsing.SplitDeclarationAndDefinition(statement);
        var (extendingRule, parents) = ParseModifiedDeclaration(declaration, extendingRules);
        var properties = ParseModifiedProperties(definition, extendingRule).ToList();
        return new GrammarRule(extendingRule.Defines, parents, properties);
    }

    private static (GrammarRule extendingRule, IEnumerable<GrammarSymbol> parentSymbols) ParseModifiedDeclaration(
        string declaration,
        IDictionary<GrammarSymbol, GrammarRule> extendingRules)
    {
        var (nonterminalText, parents) = Parsing.SplitNonterminalAndParents(declaration);
        var nonterminal = Parsing.ParseSymbol(nonterminalText);
        if (!extendingRules.Remove(nonterminal, out var extendingRule))
            throw new FormatException($"Rule not found for modification: '{nonterminal}'");
        var parentSymbols = ParseModifiedParents(parents, extendingRule);
        return (extendingRule, parentSymbols);
    }

    private static IEnumerable<GrammarSymbol> ParseModifiedParents(string? parents, GrammarRule extendingRule)
    {
        if (parents is null)
            return extendingRule.Parents;

        return ParseModifiedParentsInternal(parents, extendingRule);
    }

    private static IEnumerable<GrammarSymbol> ParseModifiedParentsInternal(string parents, GrammarRule extendingRule)
    {
        var parentsSet = extendingRule.Parents.ToHashSet();

        foreach (var parent in Parsing.SplitParents(parents))
            switch (parent[0])
            {
                case '+':
                {
                    var parentSymbol = Parsing.ParseSymbol(parent[1..]);
                    if (parentsSet.Contains(parentSymbol))
                        throw new FormatException($"Parent already exists: '{parentSymbol}'");
                    yield return parentSymbol;
                    break;
                }
                case '-':
                {
                    var parentSymbol = Parsing.ParseSymbol(parent[1..]);
                    if (!parentsSet.Remove(parentSymbol))
                        throw new FormatException($"Parent not found for removal: '{parentSymbol}'");
                    break;
                }
                default:
                    throw new FormatException($"Invalid rule parent modification: '{parent}'");
            }

        // Emit any unchanged parents
        foreach (var parent in parentsSet)
            yield return parent;
    }

    private static IEnumerable<GrammarProperty> ParseModifiedProperties(string? definition, GrammarRule extendingRule)
    {
        if (definition is null)
            return extendingRule.Properties;

        return ParseModifiedPropertiesInternal(definition, extendingRule);
    }

    private static IEnumerable<GrammarProperty> ParseModifiedPropertiesInternal(string definition, GrammarRule extendingRule)
    {
        var oldProperties = extendingRule.Properties.ToDictionary(p => p.Name);
        foreach (var property in Parsing.SplitProperties(definition))
        {
            switch (property[0])
            {
                case '+':
                {
                    var newProperty = Parsing.ParseProperty(property[1..]);
                    if (oldProperties.ContainsKey(newProperty.Name))
                        throw new FormatException($"Property already exists: '{newProperty}'");
                    yield return newProperty;
                    break;
                }
                case '*':
                {
                    var modifiedProperty = Parsing.ParseProperty(property[1..]);
                    if (!oldProperties.Remove(modifiedProperty.Name, out var originalProperty))
                        throw new FormatException($"Property not found for modification: '{modifiedProperty.Name}'");
                    if (originalProperty.Type == modifiedProperty.Type)
                        throw new FormatException($"Property not modified: '{modifiedProperty.Name}'");
                    yield return modifiedProperty;
                    break;
                }
                case '-':
                {
                    var propertyName = property[1..];
                    if (!oldProperties.Remove(propertyName, out _))
                        throw new FormatException($"Property not found for removal: '{propertyName}'");
                    break;
                }
                default:
                    throw new FormatException($"Invalid rule property modification: '{property}'");
            }
        }

        // Emit any unchanged properties
        foreach (var property in oldProperties.Values)
            yield return property;
    }

    private static GrammarRule? ParseRuleRemoval(
        string statement,
        IDictionary<GrammarSymbol, GrammarRule> extendingRules)
    {
        var symbol = Parsing.ParseSymbol(statement);
        if (!extendingRules.Remove(symbol))
            throw new FormatException($"Rule not found for removal: '{symbol}'");

        return null;
    }
}
