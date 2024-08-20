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
        var (attributeSupertypes, attributes, equations) = ParseStatements(lines);
        return new(ns, name, usingNamespaces, attributeSupertypes, attributes, equations);
    }

    private static AspectStatementsSyntax ParseStatements(
        IFixedList<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        var attributeSupertypes = new List<InheritedAttributeSupertypeSyntax>();
        var attributes = new List<AspectAttributeSyntax>();
        var equations = new List<EquationSyntax>();
        foreach (var statement in statements)
        {
            if (statement.StartsWith('='))
                equations.Add(ParseEquation(statement));
            else if (statement.StartsWith('↓') && statement.Contains("<:"))
                attributeSupertypes.Add(ParseInheritedAttributeSupertype(statement));
            else
                attributes.Add(ParseAttribute(statement));
        }
        return new(attributeSupertypes, attributes, equations);
    }

    private static InheritedAttributeSupertypeSyntax ParseInheritedAttributeSupertype(string statement)
    {
        if (!ParseOffStart(ref statement, '↓'))
            throw new ArgumentException("Not an inherited attribute statement.", nameof(statement));

        var (definition, type) = SplitTwo(statement, "<:", "Should be exactly one `<:` in: '{0}'");
        var (node, name) = SplitAtFirst(definition, ".", "Missing `.` between '*' and attribute in: '{0}'");
        if (node != "*")
            throw new FormatException("Supertype must be for all nodes (i.e. node must be '*').");
        var typeSyntax = ParseType(type);
        return new(name, typeSyntax);
    }

    private static AspectAttributeSyntax ParseAttribute(string statement)
        => statement[0] switch
        {
            '↑' => ParseSynthesizedAttribute(statement),
            '↓' => ParseInheritedAttribute(statement),
            '⮡' => ParsePreviousAttribute(statement),
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
        var isMethod = ParseOffEnd(ref definition, "()");
        var name = definition;
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(evaluationStrategy, nodeSymbol, name, isMethod, typeSyntax, defaultExpression);
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

    private static PreviousAttributeSyntax ParsePreviousAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, '⮡'))
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
        var isMethod = ParseOffEnd(ref definition, "()");
        var segments = definition.Split('.', StringSplitOptions.TrimEntries);
        var nodeSymbol = ParseSymbol(node);
        if (segments.Length == 1)
            return ParseSynthesizedEquation(evaluationStrategy, nodeSymbol, definition, isMethod, typeOverride, expression);

        var name = segments[^1];
        segments = segments[..^1];
        return ParseInheritedEquation(evaluationStrategy, nodeSymbol, segments, name, isMethod, typeOverride, expression);
    }

    private static SynthesizedAttributeEquationSyntax ParseSynthesizedEquation(
        EvaluationStrategy? evaluationStrategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        string? typeOverride,
        string? expression)
    {
        var typeOverrideSyntax = ParseType(typeOverride);
        return new(evaluationStrategy, node, name, isMethod, typeOverrideSyntax, expression);
    }

    private static InheritedAttributeEquationSyntax ParseInheritedEquation(
        EvaluationStrategy? evaluationStrategy,
        SymbolSyntax node,
        string[] selector,
        string name,
        bool isMethod,
        string? typeOverride,
        string? expression)
    {
        if (evaluationStrategy is not null)
            throw new FormatException("Inherited equations cannot have evaluation strategies.");
        if (typeOverride is not null)
            throw new FormatException("Inherited equations cannot have type overrides.");
        var selectorSyntax = ParseSelector(selector);
        return new(node, selectorSyntax, name, isMethod, expression);
    }

    private static SelectorSyntax ParseSelector(string[] selector)
    {
        if (selector.Length == 0)
            throw new ArgumentException("Must be at least one segment in selector", nameof(selector));

        var broadcast = selector[^1] == "**";
        if (broadcast)
            selector = selector[..^1];

        switch (selector.Length)
        {
            case 0:
                throw new FormatException("Broadcast selector must be applied after another selector.");
            case 1 when selector[0] == "*":
                return AllChildrenSelectorSyntax.Create(broadcast);
            case 1:
                var (child, indexer) = OptionalSplitTwo(selector[0], "[", "Too many `[` in selector '{0}'");
                if (indexer is null)
                    return new ChildSelectorSyntax(child, broadcast);
                if (!ParseOffEnd(ref indexer, "]"))
                    throw new FormatException($"Missing `]` in selector '{selector[0]}'.");
                if (indexer == "*")
                    return new ChildListSelectorSyntax(child, broadcast);
                if (int.TryParse(indexer, out var index))
                    return new ChildAtIndexSelectorSyntax(child, index, broadcast);
                return new ChildAtVariableSelectorSyntax(child, indexer, broadcast);
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

    private record AspectStatementsSyntax(
        IFixedList<InheritedAttributeSupertypeSyntax> AttributeSupertypes,
        IFixedList<AspectAttributeSyntax> Attributes,
        IFixedList<EquationSyntax> Equations)
    {
        public AspectStatementsSyntax(
            IEnumerable<InheritedAttributeSupertypeSyntax> attributeSupertypes,
            IEnumerable<AspectAttributeSyntax> attributes,
            IEnumerable<EquationSyntax> equations)
            : this(attributeSupertypes.ToFixedList(), attributes.ToFixedList(), equations.ToFixedList()) { }
    }
}
