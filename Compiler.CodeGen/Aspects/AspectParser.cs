using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Parsing;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Aspects;

public static class AspectParser
{
    public static AspectSyntax Parse(string aspectDefinition)
    {
        var lines = ParseLines(aspectDefinition).ToFixedList();

        var ns = GetRequiredConfig(lines, "namespace");
        var name = GetRequiredConfig(lines, "name");
        var usingNamespaces = ParseUsingNamespaces(lines);
        var (attributes, equations) = ParseAttributesAndEquations(lines);
        return new(ns, name, usingNamespaces, attributes, equations);
    }

    private static (IEnumerable<AspectAttributeSyntax> Attributes, IEnumerable<EquationSyntax> Equations) ParseAttributesAndEquations(
        IFixedList<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        var attributes = new List<AspectAttributeSyntax>();
        var equations = new List<EquationSyntax>();
        foreach (var statement in statements)
        {
            if (statement.StartsWith('='))
                equations.Add(ParseEquation(statement));
            else
                attributes.Add(ParseAttribute(statement));
        }
        return (attributes, equations);
    }

    private static AspectAttributeSyntax ParseAttribute(string statement)
        => statement[0] switch
        {
            '↑' => ParseSynthesizedAttribute(statement),
            '↓' => ParseInheritedAttribute(statement),
            _ => throw new($"Unknown attribute kind: {statement}"),
        };

    private static SynthesizedAttributeSyntax ParseSynthesizedAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, '↑'))
            throw new ArgumentException("Not a synthesized attribute statement.", nameof(statement));

        var (definition, defaultExpression) = OptionalSplitTwo(statement, "=>", "Too many `=>` in: '{0}'");
        (definition, var type) = SplitTwo(definition, ":", "Should be exactly one `:` in: '{0}'");
        (var node, definition) = SplitAtFirst(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        (string name, string? parameters) = SplitOffParameters(definition);
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(evaluationStrategy, nodeSymbol, name, parameters, typeSyntax, defaultExpression);
    }

    private static InheritedAttributeSyntax ParseInheritedAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, '↓'))
            throw new ArgumentException("Not an inherited attribute statement.", nameof(statement));

        var (definition, type) = SplitTwo(statement, ":", "Should be exactly one `:` in: '{0}'");
        (var node, definition) = SplitAtFirst(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        var isMethod = ParseOffEnd(ref definition, "()");
        var name = definition;
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(evaluationStrategy, nodeSymbol, name, isMethod, typeSyntax);
    }

    private static EquationSyntax ParseEquation(string statement)
    {
        if (!ParseOffStart(ref statement, '='))
            throw new ArgumentException("Not an equation.", nameof(statement));
        var (definition, expression) = OptionalSplitTwo(statement, "=>", "Too many `=>` in: '{0}'");
        (definition, var typeOverride) = OptionalSplitTwo(definition, ":", "Too many `:` in: '{0}'");
        (var node, definition) = SplitAtFirst(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        (string name, string? parameters) = SplitOffParameters(definition);
        var segments = name.Split('.', StringSplitOptions.TrimEntries);
        var nodeSymbol = ParseSymbol(node);
        if (segments.Length == 1)
            return ParseSynthesizedEquation(evaluationStrategy, nodeSymbol, name, parameters, typeOverride, expression);

        name = segments[^1];
        segments = segments[..^1];
        return ParseInheritedEquation(evaluationStrategy, nodeSymbol, segments, name, parameters, typeOverride, expression);
    }

    private static SynthesizedAttributeEquationSyntax ParseSynthesizedEquation(
        EvaluationStrategy? evaluationStrategy,
        SymbolSyntax node,
        string name,
        string? parameters,
        string? typeOverride,
        string? expression)
    {
        var typeOverrideSyntax = ParseType(typeOverride);
        return new(evaluationStrategy, node, name, parameters, typeOverrideSyntax, expression);
    }

    private static InheritedAttributeEquationSyntax ParseInheritedEquation(
        EvaluationStrategy? evaluationStrategy,
        SymbolSyntax node,
        string[] selector,
        string name,
        string? parameters,
        string? typeOverride,
        string? expression)
    {
        if (expression is not null)
            throw new FormatException("Inherited equations cannot have expressions.");
        bool isMethod = false;
        if (parameters is not null)
        {
            if (parameters != "()")
                throw new FormatException("Inherited equations cannot have parameters.");
            isMethod = true;
        }
        var selectorSyntax = ParseSelector(selector);
        return new(evaluationStrategy, node, selectorSyntax, name, isMethod, ParseType(typeOverride));
    }

    private static SelectorSyntax ParseSelector(string[] selector)
    {
        switch (selector.Length)
        {
            case 1 when selector[0] == "*":
                return AllChildrenSelectorSyntax.Instance;
            case 1 when selector[0] == "**":
                return DescendantsSelectorSyntax.Instance;
            case 1:
                var (child, indexer) = OptionalSplitTwo(selector[0], "[", "Too many `[` in selector '{0}'");
                if (indexer is null)
                    return new ChildSelectorSyntax(child);
                if (!ParseOffEnd(ref indexer, "]"))
                    throw new FormatException($"Missing `]` in selector '{selector[0]}'.");
                if (int.TryParse(indexer, out var index))
                    return new ChildAtIndexSelectorSyntax(child, index);
                return new ChildAtVariableSelectorSyntax(child, indexer);
            case 2:
                throw new NotImplementedException("Broadcast from child selectors not implemented");
            default:
                throw new FormatException($"Too many parts in selector '{string.Join('.', selector)}'.");
        }
    }

    private static EvaluationStrategy? ParseEvaluationStrategy(ref string node)
    {
        var parts = node.Split(Array.Empty<char>(),
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        switch (parts.Length)
        {
            case 0:
                throw new FormatException($"Empty node '{node}' for attribute.");
            case 1:
                return null;
            case 2:
                var strategy = parts[0];
                node = parts[1];
                return strategy switch
                {
                    "eager" => EvaluationStrategy.Eager,
                    "lazy" => EvaluationStrategy.Lazy,
                    "computed" => EvaluationStrategy.Computed,
                    _ => throw new FormatException($"Unknown evaluation strategy '{strategy}'"),
                };
            default:
                throw new FormatException($"Too many parts in node '{node}' for attribute.");
        }
    }
    private static (string name, string? parameters) SplitOffParameters(string definition)
    {
        var (name, parameters) = OptionalSplitTwo(definition, "(", "Too many `(` in: '{0}'");
        if (parameters is not null)
            // Put left paren back on parameters
            parameters = "(" + parameters;
        return (name, parameters);
    }
}
