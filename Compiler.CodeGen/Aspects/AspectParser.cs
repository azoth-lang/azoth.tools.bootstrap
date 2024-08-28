using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;
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
        var (typeDeclarations, attributeFamilies, attributes, equations, rewriteRules) = ParseStatements(lines);
        return new(ns, name, usingNamespaces, typeDeclarations, attributeFamilies, attributes, equations, rewriteRules);
    }

    private static AspectStatementsSyntax ParseStatements(
        IFixedList<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        var typeDeclarations = new List<TypeDeclarationSyntax>();
        var attributeFamilies = new List<AttributeFamilySyntax>();
        var attributes = new List<AspectAttributeSyntax>();
        var equations = new List<EquationSyntax>();
        var rewriteRules = new List<RewriteRuleSyntax>();
        foreach (var statement in statements)
        {
            if (statement.StartsWith("struct"))
                typeDeclarations.Add(ParseStructDeclaration(statement));
            else if (statement.StartsWith('↓') && statement.Contains("<:"))
                attributeFamilies.Add(ParseInheritedAttributeFamily(statement));
            else if (statement.StartsWith("↗↖") && statement.Contains(':'))
                attributeFamilies.Add(ParseAggregateAttributeFamily(statement));
            else if (statement.StartsWith('='))
                equations.Add(ParseEquation(statement));
            else if (statement.StartsWith('✎'))
                rewriteRules.Add(ParseRewriteRule(statement));
            else
                attributes.Add(ParseAttribute(statement));
        }
        return new(typeDeclarations, attributeFamilies, attributes, equations, rewriteRules);
    }

    private static TypeDeclarationSyntax ParseStructDeclaration(string statement)
    {
        var (structKeyword, name) = Bisect(statement, "Should be exactly one space in: '{0}'");
        if (structKeyword != "struct")
            throw new FormatException($"Struct declarations does not start with 'struct': '{statement}'");
        var nameSyntax = ParseSymbol(name);
        return new(true, nameSyntax);
    }

    private static InheritedAttributeFamilySyntax ParseInheritedAttributeFamily(string statement)
    {
        if (!ParseOffStart(ref statement, "↓"))
            throw new ArgumentException("Not an inherited attribute statement.", nameof(statement));

        var (definition, type) = Bisect(statement, "<:", "Should be exactly one `<:` in: '{0}'");
        string name = ParseAttributeFamily(definition);
        var typeSyntax = ParseType(type);
        return new(name, typeSyntax);
    }

    private static AggregateAttributeFamilySyntax ParseAggregateAttributeFamily(string statement)
    {
        if (!ParseOffStart(ref statement, "↗↖"))
            throw new ArgumentException("Not an aggregate attribute family statement.", nameof(statement));

        (var definition, statement) = Bisect(statement, ":", "Should be exactly one `:` in: '{0}'");
        var name = ParseAttributeFamily(definition);
        (statement, var doneMethod) = SplitOffEnd(statement, "Should end in done method: '{0}'");
        (statement, var doneKeyword) = SplitOffEnd(statement, "Missing done keyword: '{0}'");
        if (doneKeyword != "done") throw new FormatException($"Expected 'done', found: '{doneKeyword}'");
        (statement, var aggregateMethod) = OptionalSplitOffEnd(statement, "with");
        (statement, var constructExpression) = OptionalSplitOffEnd(statement, "=>");
        var (type, fromType) = Bisect(statement, "from", "Should be exactly one `from` in: '{0}'");
        var typeSyntax = ParseType(type);
        var fromTypeSyntax = ParseType(fromType);
        return new(name, typeSyntax, fromTypeSyntax, constructExpression, aggregateMethod, doneMethod);
    }

    private static string ParseAttributeFamily(string definition)
    {
        var (node, name) = SplitOffStart(definition, ".", "Missing `.` between '*' and attribute in: '{0}'");
        if (node != "*") throw new FormatException("Family must be for all nodes (i.e. node must be '*').");
        return name;
    }

    private static AspectAttributeSyntax ParseAttribute(string statement)
        => statement[0] switch
        {
            '↑' => ParseSynthesizedAttribute(statement),
            '↓' => ParseInheritedAttribute(statement),
            '⮡' => ParsePreviousAttribute(statement),
            '+' => ParseIntertypeMethodAttribute(statement),
            _ => throw new($"Unknown attribute kind: {statement}"),
        };

    private static SynthesizedAttributeSyntax ParseSynthesizedAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "↑"))
            throw new ArgumentException("Not a synthesized attribute statement.", nameof(statement));

        var (definition, defaultExpression) = OptionalSplitTwo(statement, "=>", "Too many `=>` in: '{0}'");
        (definition, var type) = Bisect(definition, ":", "Should be exactly one `:` in: '{0}'");
        (var node, definition) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var isChild = ParseIsChild(ref node);
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        var isMethod = ParseOffEnd(ref definition, "()");
        var name = definition;
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(isChild, evaluationStrategy, nodeSymbol, name, isMethod, typeSyntax, defaultExpression);
    }

    private static InheritedAttributeSyntax ParseInheritedAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "↓"))
            throw new ArgumentException("Not an inherited attribute statement.", nameof(statement));

        var (definition, type) = Bisect(statement, ":", "Should be exactly one `:` in: '{0}'");
        (var node, definition) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        var isMethod = ParseOffEnd(ref definition, "()");
        var name = definition;
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(evaluationStrategy, nodeSymbol, name, isMethod, typeSyntax);
    }

    private static PreviousAttributeSyntax ParsePreviousAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "⮡"))
            throw new ArgumentException("Not an inherited attribute statement.", nameof(statement));

        var (definition, type) = Bisect(statement, ":", "Should be exactly one `:` in: '{0}'");
        (var node, definition) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        var isMethod = ParseOffEnd(ref definition, "()");
        var name = definition;
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(evaluationStrategy, nodeSymbol, name, isMethod, typeSyntax);
    }

    private static IntertypeMethodAttributeSyntax ParseIntertypeMethodAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "+"))
            throw new ArgumentException("Not an intertype method attribute statement.", nameof(statement));

        var (definition, defaultExpression) = OptionalSplitTwo(statement, "=>", "Should be exactly one `=>` in: '{0}'");
        (definition, var type) = Bisect(definition, ":", "Should be exactly one `:` in: '{0}'");
        (var node, definition) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var parameters = ParseOffParameters(ref definition);
        var name = definition;
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(nodeSymbol, name, parameters, typeSyntax, defaultExpression);
    }

    private static string ParseOffParameters(ref string definition)
    {
        (definition, var parameters) = SplitOffStart(definition, "(", "Missing `(` the start parameters in: '{0}'");
        if (!ParseOffEnd(ref parameters, ")"))
            throw new FormatException("Missing `)` at the end of parameters.");
        return parameters;
    }

    private static EquationSyntax ParseEquation(string statement)
    {
        if (!ParseOffStart(ref statement, "="))
            throw new ArgumentException("Not an equation.", nameof(statement));
        var (definition, expression) = OptionalSplitTwo(statement, "=>", "Too many `=>` in: '{0}'");
        (definition, var typeOverride) = OptionalSplitTwo(definition, ":", "Too many `:` in: '{0}'");
        (var node, definition) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        var parameters = TryParseOffParameters(ref definition);
        var isMethod = parameters is not null;
        var segments = definition.Split('.', StringSplitOptions.TrimEntries);
        var nodeSymbol = ParseSymbol(node);
        if (segments.Length == 1)
        {
            if (isMethod && parameters!.Length > 0)
                return ParseIntertypeMethodEquation(evaluationStrategy, nodeSymbol, definition, parameters, typeOverride, expression);
            return ParseSynthesizedEquation(evaluationStrategy, nodeSymbol, definition, isMethod, typeOverride, expression);
        }
        var name = segments[^1];
        segments = segments[..^1];
        return ParseInheritedEquation(evaluationStrategy, nodeSymbol, segments, name, isMethod, typeOverride, expression);
    }

    private static string? TryParseOffParameters(ref string definition)
    {
        (definition, var parameters) = OptionalSplitOffEnd(definition, "(");
        if (parameters is null) return null;
        if (!ParseOffEnd(ref parameters, ")")) throw new FormatException("Missing `)` at the end of parameters.");
        return parameters;
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
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string[] selector,
        string name,
        bool isMethod,
        string? typeOverride,
        string? expression)
    {
        if (strategy is not null)
            throw new FormatException("Inherited equations cannot have evaluation strategies.");
        if (typeOverride is not null)
            throw new FormatException("Inherited equations cannot have type overrides.");
        var selectorSyntax = ParseSelector(selector);
        return new(node, selectorSyntax, name, isMethod, expression);
    }

    private static IntertypeMethodEquationSyntax ParseIntertypeMethodEquation(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        string parameters,
        string? typeOverride,
        string? expression)
    {
        if (strategy is not null)
            throw new FormatException("Intertype method equations cannot have evaluation strategies.");
        if (expression is null)
            throw new FormatException("Intertype method equations must have an expression.");
        return new(node, name, parameters, ParseType(typeOverride), expression);
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

    private static bool ParseIsChild(ref string node)
    {
        var (keyword, rest) = OptionalSplitOffStart(node);
        if (keyword == "child")
        {
            node = rest;
            return true;
        }
        return false;
    }

    private static EvaluationStrategy? ParseEvaluationStrategy(ref string node)
    {
        (var strategy, node) = OptionalSplitOffStart(node);
        return strategy switch
        {
            null => null,
            "eager" => EvaluationStrategy.Eager,
            "lazy" => EvaluationStrategy.Lazy,
            "computed" => EvaluationStrategy.Computed,
            _ => throw new FormatException($"Unknown evaluation strategy '{strategy}'"),
        };
    }

    private static RewriteRuleSyntax ParseRewriteRule(string statement)
    {
        if (!ParseOffStart(ref statement, "✎"))
            throw new ArgumentException("Not a rewrite rule.", nameof(statement));

        var (node, name) = OptionalSplitOffEnd(statement);
        return new(ParseSymbol(node), name);
    }

    private record AspectStatementsSyntax(
        IFixedSet<TypeDeclarationSyntax> TypeDeclarations,
        IFixedSet<AttributeFamilySyntax> AttributeFamilies,
        IFixedList<AspectAttributeSyntax> Attributes,
        IFixedList<EquationSyntax> Equations,
        IFixedList<RewriteRuleSyntax> RewriteRules)
    {
        public AspectStatementsSyntax(
            IEnumerable<TypeDeclarationSyntax> typeDeclarations,
            IEnumerable<AttributeFamilySyntax> attributeFamilies,
            IEnumerable<AspectAttributeSyntax> attributes,
            IEnumerable<EquationSyntax> equations,
            IEnumerable<RewriteRuleSyntax> rewriteRules)
            : this(typeDeclarations.ToFixedSet(), attributeFamilies.ToFixedSet(),
                attributes.ToFixedList(), equations.ToFixedList(), rewriteRules.ToFixedList())
        { }
    }
}
