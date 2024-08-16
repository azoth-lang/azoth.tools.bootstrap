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

    private static (IEnumerable<AttributeSyntax> Attributes, IEnumerable<EquationSyntax> Equations) ParseAttributesAndEquations(
        IFixedList<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        var attributes = new List<AttributeSyntax>();
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

    private static AttributeSyntax ParseAttribute(string statement)
        => statement[0] switch
        {
            '↑' => ParseSynthesizedAttribute(statement),
            _ => throw new Exception($"Unknown attribute kind: {statement}"),
        };

    private static SynthesizedAttributeSyntax ParseSynthesizedAttribute(string statement)
    {
        if (!ParseOffStart(ref statement, '↑'))
            throw new ArgumentException("Not a synthesized attribute statement.", nameof(statement));
        statement = statement[1..].Trim();

        var (definition, defaultExpression) = OptionalSplitTwo(statement, "=>", "Too many `=>` in: '{0}'");
        (definition, var type) = SplitTwo(definition, ":", "Should be exactly one `:` in: '{0}'");
        (var node, definition) = SplitAtFirst(definition, ".", "Missing `.` between node and attribute in: '{0}'");
        (string name, string? parameters) = SplitOffParameters(definition);
        var typeSyntax = ParseType(type);
        var nodeSymbol = new SymbolSyntax(node);
        return new(nodeSymbol, name, parameters, typeSyntax, defaultExpression);
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
        var typeOverrideSyntax = ParseType(typeOverride);
        var nodeSymbol = new SymbolSyntax(node);
        return new SynthesizedAttributeEquationSyntax(evaluationStrategy, nodeSymbol, name, parameters, typeOverrideSyntax, expression);
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
