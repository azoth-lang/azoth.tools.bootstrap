using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Snippets;
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
        var (typeDeclarations, snippets, attributeFamilies, attributes, equations, rewriteRules) = ParseStatements(lines);
        return new(ns, name, usingNamespaces, typeDeclarations, snippets, attributeFamilies,
            attributes, equations, rewriteRules);
    }

    private static AspectStatementsSyntax ParseStatements(
        IFixedList<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        var typeDeclarations = new List<TypeDeclarationSyntax>();
        var snippets = new List<SnippetSyntax>();
        var attributeFamilies = new List<AttributeFamilySyntax>();
        var attributes = new List<AspectAttributeSyntax>();
        var equations = new List<EquationSyntax>();
        var rewriteRules = new List<RewriteRuleSyntax>();
        foreach (var statement in statements)
        {
            if (statement.StartsWith("struct") || statement.StartsWith("type"))
                typeDeclarations.Add(ParseTypeDeclaration(statement));
            else if (statement.StartsWith('+') && statement.Contains("new") && statement.Contains("Validate"))
                snippets.Add(ParseConstructorArgumentValidation(statement));
            else if (statement.StartsWith('↓') && statement.Contains("<:"))
                attributeFamilies.Add(ParseInheritedAttributeFamily(statement));
            else if (statement.StartsWith('⮡') && statement.Contains(':'))
                attributeFamilies.Add(ParsePreviousAttributeFamily(statement));
            else if (statement.StartsWith("↗↖") && statement.Contains(':'))
                attributeFamilies.Add(ParseAggregateAttributeFamily(statement));
            else if (statement.StartsWith('='))
                equations.Add(ParseEquation(statement));
            else if (statement.StartsWith('✎'))
                rewriteRules.Add(ParseRewriteRule(statement));
            else
                attributes.Add(ParseAttribute(statement));
        }
        return new(typeDeclarations, snippets, attributeFamilies, attributes, equations, rewriteRules);
    }

    private static ConstructorArgumentValidationSyntax ParseConstructorArgumentValidation(string statement)
    {
        if (!ParseOffStart(ref statement, "+"))
            throw new ArgumentException("Not a constructor argument validation statement.", nameof(statement));
        (var node, statement) = SplitOffStart(statement, ".", "Missing `.` in: '{0}'");
        var (newKeyword, validateKeyword) = Bisect(statement, ".", "Should be exactly one `.` in: '{0}'");
        if (newKeyword != "new")
            throw new FormatException($"Expected 'new', found: '{newKeyword}'");
        if (validateKeyword != "Validate")
            throw new FormatException($"Expected 'Validate', found: '{validateKeyword}'");
        return new(ParseSymbol(node));
    }

    private static TypeDeclarationSyntax ParseTypeDeclaration(string statement)
    {
        (var keyword, statement) = SplitOffStart(statement, "Missing whitespace in: '{0}'");
        var isValueType = keyword switch
        {
            "struct" => true,
            "type" => false,
            _ => throw new FormatException($"Type declarations does not start with 'struct' or 'type': '{statement}'"),
        };
        var (name, supertypes) = OptionalBisect(statement, "<:", "Too many `<:` in: '{0}'");
        var nameSyntax = ParseSymbol(name);
        var supertypeSyntax = ParseSupertypes(supertypes);
        return new(isValueType, nameSyntax, supertypeSyntax);
    }

    private static InheritedAttributeFamilySyntax ParseInheritedAttributeFamily(string statement)
    {
        if (!ParseOffStart(ref statement, "↓"))
            throw new ArgumentException("Not an inherited attribute family statement.", nameof(statement));

        var (definition, type) = Bisect(statement, "<:", "Should be exactly one `<:` in: '{0}'");
        (var modifier, definition) = OptionalSplitOffStart(definition);
        bool isStable = modifier switch
        {
            "stable" => true,
            null => false,
            _ => throw new FormatException(
                $"Unexpected modifier '{modifier}' on inherited attribute family '{definition}'.")
        };
        string name = ParseAttributeFamily(definition);
        var typeSyntax = ParseType(type);
        return new(isStable, name, typeSyntax);
    }

    private static PreviousAttributeFamilySyntax ParsePreviousAttributeFamily(string statement)
    {
        if (!ParseOffStart(ref statement, "⮡"))
            throw new ArgumentException("Not an previous attribute family statement.", nameof(statement));

        var (definition, type) = Bisect(statement, ":", "Should be exactly one `:` in: '{0}'");
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
            '⟳' => ParseCircularAttribute(statement),
            '↓' => ParseInheritedAttribute(statement),
            '⮡' => ParsePreviousAttribute(statement),
            '+' => ParseIntertypeMethodAttribute(statement),
            '↗' => ParseAggregateAttribute(statement),
            '→' => ParseCollectionAttribute(statement),
            _ => throw new($"Unknown attribute kind: {statement}"),
        };

    private static SynthesizedAttributeSyntax ParseSynthesizedAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "↑"))
            throw new ArgumentException("Not a synthesized attribute statement.", nameof(statement));

        var (definition, defaultExpression) = OptionalBisect(statement, "=>", "Too many `=>` in: '{0}'");
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

    private static CircularAttributeSyntax ParseCircularAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "⟳"))
            throw new ArgumentException("Not a circular attribute statement.", nameof(statement));

        var (definition, initialExpression) = OptionalBisect(statement, "initial", "Too many `initial` in: '{0}'");
        if (initialExpression is not null)
            if (!ParseOffStart(ref initialExpression, "=>"))
                throw new FormatException("Initial expression must start with `=>`.");
        (definition, var defaultExpression) = OptionalBisect(definition, "=>", "Too many `=>` in: '{0}'");
        (definition, var type) = Bisect(definition, ":", "Should be exactly one `:` in: '{0}'");
        var (node, name) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(nodeSymbol, typeSyntax, name, defaultExpression, initialExpression);
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

        var (definition, type) = OptionalBisect(statement, ":", "Should be exactly one `:` in: '{0}'");
        if (type is not null)
            throw new FormatException($"Previous attributes cannot have type: {statement}");
        (var node, definition) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        var isMethod = ParseOffEnd(ref definition, "()");
        var name = definition;
        // TODO use the type syntax
        var typeSyntax = ParseType(type);
        var nodeSymbol = ParseSymbol(node);
        return new(evaluationStrategy, nodeSymbol, name, isMethod);
    }

    private static IntertypeMethodAttributeSyntax ParseIntertypeMethodAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "+"))
            throw new ArgumentException("Not an intertype method attribute statement.", nameof(statement));

        var (definition, defaultExpression) = OptionalBisect(statement, "=>", "Should be exactly one `=>` in: '{0}'");
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

    private static AggregateAttributeSyntax ParseAggregateAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "↗↖"))
            throw new ArgumentException("Not an aggregate attribute statement.", nameof(statement));

        var (node, attribute) = Bisect(statement, ".", "Should be exactly one `.` in: '{0}'");
        return new(ParseSymbol(node), attribute);
    }

    private static CollectionAttributeSyntax ParseCollectionAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, "→*←"))
            throw new ArgumentException("Not a collection attribute statement.", nameof(statement));

        (var definition, statement) = Bisect(statement, ":", "Should be exactly one `:` in: '{0}'");
        var (node, name) = Bisect(definition, ".", "Should be exactly one `.` in: '{0}'");
        (statement, var doneMethod) = SplitOffEnd(statement, "Should end in done method: '{0}'");
        (statement, var doneKeyword) = SplitOffEnd(statement, "Missing done keyword: '{0}'");
        if (doneKeyword != "done") throw new FormatException($"Expected 'done', found: '{doneKeyword}'");
        (statement, var constructExpression) = OptionalSplitOffEnd(statement, "=>");
        (statement, var fromType) = Bisect(statement, "from", "Should be exactly one `from` in: '{0}'");
        var (type, root) = OptionalSplitOffEnd(statement, "root");
        var nodeSyntax = ParseSymbol(node);
        var typeSyntax = ParseType(type);
        var fromTypeSyntax = ParseType(fromType);
        return new(nodeSyntax, name, typeSyntax, ParseSymbol(root), fromTypeSyntax, constructExpression, doneMethod);
    }

    private static EquationSyntax ParseEquation(string statement)
    {
        if (!ParseOffStart(ref statement, "="))
            throw new ArgumentException("Not an equation.", nameof(statement));
        var (definition, expression) = OptionalBisect(statement, "=>", "Too many `=>` in: '{0}'");
        (definition, var typeOverride) = OptionalBisect(definition, ":", "Too many `:` in: '{0}'");
        (var node, definition) = SplitOffStart(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        var evaluationStrategy = ParseEvaluationStrategy(ref node);
        var parameters = TryParseOffParameters(ref definition);
        var isMethod = parameters is not null;
        var nodeSymbol = ParseSymbol(node);
        var segments = definition.Split('.', StringSplitOptions.TrimEntries);
        if (segments.Length == 1)
        {
            if (isMethod && parameters!.Length > 0)
                return ParseIntertypeMethodEquation(evaluationStrategy, nodeSymbol, definition, parameters, typeOverride, expression);
            return ParseLocalEquation(evaluationStrategy, nodeSymbol, definition, isMethod, typeOverride, expression);
        }
        if (segments[0] == "→*")
            return ParseCollectionEquation(evaluationStrategy, nodeSymbol, definition, isMethod, typeOverride, expression);
        var name = segments[^1];
        segments = segments[..^1];
        if (segments.Length == 1)
            switch (segments[0])
            {
                case "↑":
                    return ParseAggregateEquation(evaluationStrategy, nodeSymbol, name, isMethod, typeOverride, expression);
                case "⮡":
                    return ParsePreviousEquation(evaluationStrategy, nodeSymbol, name, isMethod, typeOverride, expression);
            }

        return ParseInheritedEquation(evaluationStrategy, nodeSymbol, segments, name, isMethod, typeOverride, expression);
    }

    private static string? TryParseOffParameters(ref string definition)
    {
        (definition, var parameters) = OptionalSplitOffEnd(definition, "(");
        if (parameters is null) return null;
        if (!ParseOffEnd(ref parameters, ")")) throw new FormatException("Missing `)` at the end of parameters.");
        return parameters;
    }

    private static LocalAttributeEquationSyntax ParseLocalEquation(
        EvaluationStrategy? evaluationStrategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        string? typeOverride,
        string? expression)
    {
        if (typeOverride is not null)
            throw new FormatException("Local attribute equations cannot have type overrides.");
        return new(evaluationStrategy, node, name, isMethod, expression);
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

    private static PreviousAttributeEquationSyntax ParsePreviousEquation(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        string? typeOverride,
        string? expression)
    {
        if (strategy is not null)
            throw new FormatException("Previous equations cannot have evaluation strategies.");
        if (typeOverride is not null)
            throw new FormatException("Previous equations cannot have type overrides.");
        return new(node, name, isMethod, expression);
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
        if (typeOverride is not null)
            throw new FormatException("Intertype method equations cannot have an types overrides.");
        if (expression is null)
            throw new FormatException("Intertype method equations must have an expression.");
        return new(node, name, parameters, expression);
    }

    private static AggregateAttributeEquationSyntax ParseAggregateEquation(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        string? typeOverride,
        string? expression)
    {
        if (strategy is not null)
            throw new FormatException("Aggregate equations cannot have evaluation strategies.");
        if (isMethod)
            throw new FormatException("Aggregate equations cannot be methods.");
        if (typeOverride is not null)
            throw new FormatException("Aggregate equations cannot have an types overrides.");
        if (expression is not null)
            throw new FormatException("Aggregate equations cannot have an expression.");

        return new(node, name);
    }

    private static CollectionAttributeEquationSyntax ParseCollectionEquation(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string definition,
        bool isMethod,
        string? typeOverride,
        string? expression)
    {
        if (strategy is not null)
            throw new FormatException("Collection equations cannot have evaluation strategies.");
        if (isMethod)
            throw new FormatException("Collection equations cannot be methods.");
        if (typeOverride is not null)
            throw new FormatException("Collection equations cannot have an types overrides.");
        if (expression is not null)
            throw new FormatException("Collection equations cannot have an expression.");

        (var selector, definition) = SplitOffStart(definition, ".", "Missing `.` between selector and target in: '{0}");
        if (selector != "→*")
            throw new FormatException("Collection equations must start with `→*`.");
        (definition, var targetExpression) = OptionalSplitOffEnd(definition, "for");
        var isForEach = targetExpression is not null && ParseOffStart(ref targetExpression, "each");
        var (target, name) = Bisect(definition, ".", "Should be exactly one `.` in: '{0}'");
        var targetNode = ParseSymbol(target);
        return new(node, targetNode, name, isForEach, targetExpression);
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
                var (child, indexer) = OptionalBisect(selector[0], "[", "Too many `[` in selector '{0}'");
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

        var (target, rest) = SplitOffStart(statement, "Missing whitespace in: '{0}'");
        var targetNode = ParseSymbol(target);
        var (name, kind, rewriteTo) = ParseRewriteNameAndKind(rest);
        return new(targetNode, name, kind, ParseSymbol(rewriteTo));
    }

    private static (string?, RewriteKind, string?) ParseRewriteNameAndKind(string rest)
    {
        (var nameOrKind, rest) = SplitOffStart(rest, "Missing whitespace in: '{0}'");
        if (ParseRewriteKind(nameOrKind) is { } kind)
            return (null, kind, rest);

        var name = nameOrKind;
        var (kindString, rewriteTo) = OptionalSplitOffEnd(rest);
        kind = ParseRewriteKind(kindString)
               ?? throw new FormatException($"Invalid rewrite kind '{kindString}'.");
        return (name, kind, rewriteTo);
    }

    private static RewriteKind? ParseRewriteKind(string kind)
        => kind switch
        {
            "insert" => RewriteKind.InsertAbove,
            "replace_with" => RewriteKind.Replace,
            "rewrite" => RewriteKind.RewriteSubtree,
            _ => null
        };

    private record AspectStatementsSyntax(
        IFixedSet<TypeDeclarationSyntax> TypeDeclarations,
        IFixedList<SnippetSyntax> Snippets,
        IFixedSet<AttributeFamilySyntax> AttributeFamilies,
        IFixedList<AspectAttributeSyntax> Attributes,
        IFixedList<EquationSyntax> Equations,
        IFixedList<RewriteRuleSyntax> RewriteRules)
    {
        public AspectStatementsSyntax(
            IEnumerable<TypeDeclarationSyntax> typeDeclarations,
            IEnumerable<SnippetSyntax> snippets,
            IEnumerable<AttributeFamilySyntax> attributeFamilies,
            IEnumerable<AspectAttributeSyntax> attributes,
            IEnumerable<EquationSyntax> equations,
            IEnumerable<RewriteRuleSyntax> rewriteRules)
            : this(typeDeclarations.ToFixedSet(), snippets.ToFixedList(), attributeFamilies.ToFixedSet(),
                attributes.ToFixedList(), equations.ToFixedList(), rewriteRules.ToFixedList())
        { }
    }
}
