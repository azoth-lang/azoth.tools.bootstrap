using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Passes;

internal static class PassParser
{
    public static PassNode ParsePass(string input)
    {
        var lines = Parsing.ParseLines(input).ToFixedList();

        var name = Parsing.GetRequiredConfig(lines, "name");
        var ns = Parsing.GetRequiredConfig(lines, "namespace");
        var usingNamespaces = Parsing.ParseUsingNamespaces(lines);
        var fromName = Parsing.ParseSymbol(Parsing.GetConfig(lines, "from"));
        var toName = Parsing.ParseSymbol(Parsing.GetConfig(lines, "to"));
        var context = Parsing.GetConfig(lines, "context");
        var (fromContext, toContext) = ParseContext(context);

        var hasFromLanguage = fromName is not null && !fromName.IsQuoted;
        var hasToLanguage = toName is not null && !toName.IsQuoted;
        var transforms = ParseTransforms(lines, hasFromLanguage, hasToLanguage).ToFixedList();

        return new PassNode(ns, name, usingNamespaces, fromContext, toContext, fromName, toName, transforms);
    }

    private static (SymbolNode?, SymbolNode?) ParseContext(string? context)
    {
        if (context is null)
            return (null, null);

        var parts = context.Split("->");
        switch (parts.Length)
        {
            case 1:
                var contextSymbol = new SymbolNode(parts[0].Trim(), true);
                return (contextSymbol, contextSymbol);
            case 2:
                return (new SymbolNode(parts[0].Trim(), true), new SymbolNode(parts[1].Trim(), true));
            default:
                throw new FormatException($"Invalid context format '{context}'");
        }
    }

    private static IEnumerable<TransformNode> ParseTransforms(
        IFixedList<string> lines,
        bool hasFromLanguage,
        bool hasToLanguage)
    {
        var transforms = Parsing.ParseToStatements(lines).ToFixedList();
        foreach (var transform in transforms)
            yield return ParseTransform(transform, hasFromLanguage, hasToLanguage);
    }

    private static TransformNode ParseTransform(string transform, bool hasFromLanguage, bool hasToLanguage)
    {
        (transform, var autoGenerate) = ParseAutoGenerate(transform);
        var parts = transform.Split("->");
        switch (parts.Length)
        {
            case 1:
            {
                if (hasToLanguage)
                    throw new FormatException($"Transform must have a return type because transforming to a language. '{transform}'");
                var transformFrom = ParseTransformFrom(parts[0], hasFromLanguage);
                return new TransformNode(transformFrom, FixedList.Empty<ParameterNode>(), autoGenerate);
            }
            case 2:
            {
                var transformFrom = ParseTransformFrom(parts[0], hasFromLanguage);
                var defaultSymbol = hasFromLanguage ? transformFrom[0].Type.Symbol : null;
                var transformTo = ParseTransformTo(parts[1], hasToLanguage, defaultSymbol);
                return new TransformNode(transformFrom, transformTo, autoGenerate);
            }
            default:
                throw new FormatException($"Invalid transform format '{transform}'");
        }
    }

    private static (string, bool) ParseAutoGenerate(string transform)
    {
        var parts = transform.Split("=");
        if (parts.Length == 1)
            return (transform, false);
        if (parts.Length == 2)
            return (parts[0].Trim(), true);
        throw new FormatException($"Invalid transform format '{transform}'");
    }

    private static IFixedList<ParameterNode> ParseTransformFrom(string parameters, bool hasFromLanguage)
    {
        var defaultFirstName = hasFromLanguage ? "from" : null;
        var parameterSyntax = ParseParameters(parameters, defaultFirstName).ToFixedList();
        if (hasFromLanguage)
        {
            if (parameterSyntax.Count == 0)
                throw new FormatException($"Transform must have parameters because transforming from a language. '{parameters}'");
            if (parameterSyntax[0].Type.Symbol.IsQuoted)
                throw new FormatException($"First parameter must not be quoted because transforming from a language. '{parameters}'");
        }
        return parameterSyntax;
    }

    private static IEnumerable<ParameterNode> ParseParameters(string parameters, string? defaultFirstName)
    {
        var properties = SplitParameters(parameters);
        var defaultName = defaultFirstName;
        foreach (var property in properties)
        {
            yield return Parsing.ParseParameter(property, defaultName);
            defaultName = null;
        }
    }

    public static IEnumerable<string> SplitParameters(string definition)
        => definition.SplitOrEmpty(',').Select(p => p.Trim());

    private static IFixedList<ParameterNode> ParseTransformTo(
        string returnValues,
        bool hasToLanguage,
        SymbolNode? defaultSymbol)
    {
        if (defaultSymbol is not null)
            returnValues = returnValues.Replace("~", defaultSymbol.ToString());
        var defaultFirstName = hasToLanguage ? "to" : null;
        var returnSyntax = ParseParameters(returnValues, defaultFirstName).ToFixedList();
        if (hasToLanguage)
        {
            if (returnSyntax.Count == 0)
                throw new FormatException($"Transform must have return values because transforming to a language. '{returnValues}'");
            if (returnSyntax[0].Type.Symbol.IsQuoted)
                throw new FormatException($"First return value must not be quoted because transforming to a language. '{returnValues}'");
        }
        return returnSyntax;
    }
}
