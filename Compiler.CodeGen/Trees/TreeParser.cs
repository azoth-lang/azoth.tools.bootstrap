using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

internal static class TreeParser
{
    public static Grammar ReadGrammarConfig(string grammar)
    {
        var lines = Parsing.ParseLines(grammar).ToFixedList();

        var ns = Parsing.GetConfig(lines, "namespace");
        var baseType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "base"));
        var prefix = Parsing.GetConfig(lines, "prefix") ?? "";
        var suffix = Parsing.GetConfig(lines, "suffix") ?? "";
        var listType = Parsing.GetConfig(lines, "list") ?? "List";
        var usingNamespaces = Parsing.GetUsingNamespaces(lines);
        var rules = ParseRules(lines).Select(r => WithDefaultBaseType(r, baseType));
        return new Grammar(ns, baseType, prefix, suffix, listType, usingNamespaces, rules);
    }

    private static GrammarRule WithDefaultBaseType(GrammarRule rule, GrammarSymbol? baseType)
    {
        if (baseType is null
            || rule.Parents.Any()
            || rule.Nonterminal == baseType) return rule;
        return new GrammarRule(rule.Nonterminal, baseType.YieldValue(), rule.Properties);
    }

    private static IEnumerable<GrammarRule> ParseRules(IEnumerable<string> lines)
    {
        var statements = Parsing.ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
            yield return ParseRule(statement);
    }

    private static GrammarRule ParseRule(string statement)
    {
        var equalSplit = statement.Split('=');
        if (equalSplit.Length > 2) throw new FormatException($"Too many equal signs on line: '{statement}'");
        var declaration = equalSplit[0];
        var (nonterminal, parents) = ParseDeclaration(declaration);
        var definition = equalSplit.Length == 2 ? equalSplit[1].Trim() : null;
        var properties = ParseDefinition(definition).ToList();
        if (properties.Select(p => p.Name).Distinct().Count() != properties.Count)
            throw new FormatException($"Rule for {nonterminal} contains duplicate property definitions");

        return new GrammarRule(nonterminal, parents, properties);
    }

    private static IEnumerable<GrammarProperty> ParseDefinition(string? definition)
    {
        if (definition is null) yield break;

        var properties = definition.SplitOrEmpty(' ').Where(v => !string.IsNullOrWhiteSpace(v));
        foreach (var property in properties)
        {
            var trimmedProperty = property;

            var isRef = trimmedProperty.StartsWith('&');
            trimmedProperty = isRef ? trimmedProperty[1..] : trimmedProperty;

            var isOptional = trimmedProperty.EndsWith('?');
            trimmedProperty = isOptional ? trimmedProperty[..^1] : trimmedProperty;

            var isList = trimmedProperty.EndsWith('*');
            trimmedProperty = isList ? trimmedProperty[..^1] : trimmedProperty;

            var parts = trimmedProperty.Split(':').Select(p => p.Trim()).ToArray();

            switch (parts.Length)
            {
                case 1:
                {
                    var name = parts[0];
                    var grammarType = new GrammarType(Parsing.ParseSymbol(name), isRef, isList, isOptional);
                    yield return new GrammarProperty(name, grammarType);
                }
                break;
                case 2:
                {
                    var name = parts[0];
                    var type = parts[1];
                    var grammarType = new GrammarType(Parsing.ParseSymbol(type), isRef, isList, isOptional);
                    yield return new GrammarProperty(name, grammarType);
                }
                break;
                default:
                    throw new FormatException($"Too many colons in definition: '{definition}'");
            }
        }
    }

    private static (GrammarSymbol nonterminal, IEnumerable<GrammarSymbol> parents) ParseDeclaration(string declaration)
    {
        var declarationSplit = declaration.SplitOrEmpty(':');
        if (declarationSplit.Count > 2) throw new FormatException($"Too many colons in declaration: '{declaration}'");
        var nonterminal = Parsing.ParseSymbol(declarationSplit[0].Trim());
        var parents = declarationSplit.Count == 2 ? declarationSplit[1] : null;
        var parentSymbols = ParseParents(parents);
        return (nonterminal, parentSymbols);
    }

    private static IEnumerable<GrammarSymbol> ParseParents(string? parents)
    {
        if (parents is null) return Enumerable.Empty<GrammarSymbol>();

        return parents
               .Split(',')
               .Select(p => p.Trim())
               .Select(p => Parsing.ParseSymbol(p));
    }
}
