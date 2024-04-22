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

    private static (string?, string?) ParseContext(string? context)
    {
        if (context is null)
            return (null, null);

        var parts = context.Split("->");
        switch (parts.Length)
        {
            case 1:
                var contextName = parts[0].Trim();
                return (contextName, contextName);
            case 2:
                return (parts[0].Trim(), parts[1].Trim());
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
                var (from, additionalParameters) = ParseTransformFrom(parts[0], hasFromLanguage);
                return new TransformNode(from, additionalParameters, null, FixedList.Empty<ParameterNode>(), autoGenerate);
            }
            case 2:
            {
                var (from, additionalParameters) = ParseTransformFrom(parts[0], hasFromLanguage);
                var defaultSymbol = from?.Type.Symbol;
                var (to, additionalReturnValues) = ParseTransformTo(parts[1], hasToLanguage, defaultSymbol);
                return new TransformNode(from, additionalParameters, to, additionalReturnValues, autoGenerate);
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

    private static (ParameterNode?, IFixedList<ParameterNode>) ParseTransformFrom(string parameters, bool hasFromLanguage)
    {
        var defaultFirstName = hasFromLanguage ? "from" : null;
        var (fromSyntax, additionalParametersSyntax) = ParseParameters(parameters, hasFromLanguage, defaultFirstName);
        return (fromSyntax, additionalParametersSyntax);
    }

    private static (ParameterNode?, IFixedList<ParameterNode>) ParseParameters(
        string parameters,
        bool hasLanguage,
        string? defaultFirstNodeName)
    {
        var allParameters = ParseAllParameters(parameters, defaultFirstNodeName).ToFixedList();
        if (!hasLanguage || (allParameters.FirstOrDefault()?.Type.Symbol.IsQuoted ?? true))
            return (null, allParameters);
        return (allParameters[0], allParameters.Skip(1).ToFixedList());
    }

    private static IEnumerable<ParameterNode> ParseAllParameters(string parameters, string? defaultFirstNodeName)
    {
        var properties = SplitParameters(parameters);
        var defaultName = defaultFirstNodeName;
        foreach (var property in properties)
        {
            yield return Parsing.ParseParameter(property, defaultName);
            defaultName = null;
        }
    }

    public static IEnumerable<string> SplitParameters(string definition)
        => definition.SplitOrEmpty(',').Select(p => p.Trim());

    private static (ParameterNode?, IFixedList<ParameterNode>) ParseTransformTo(
        string returnValues,
        bool hasToLanguage,
        SymbolNode? defaultSymbol)
    {
        if (defaultSymbol is not null)
            returnValues = returnValues.Replace("~", defaultSymbol.ToString());
        var defaultFirstName = hasToLanguage ? "to" : null;
        var (toSyntax, additionalReturnValueSyntax) = ParseParameters(returnValues, hasToLanguage, defaultFirstName);
        return (toSyntax, additionalReturnValueSyntax);
    }
}
