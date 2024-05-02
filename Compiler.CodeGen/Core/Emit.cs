using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types.Type;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(Rule rule, string indent = "")
    {
        var children = rule.DerivedRules;
        if (!children.Any()) return "";
        var builder = new StringBuilder();
        builder.Append(indent);
        builder.AppendLine("[Closed(");
        bool first = true;
        foreach (var child in children)
        {
            if (first)
                first = false;
            else
                builder.AppendLine(",");
            builder.Append(indent);
            builder.Append($"    typeof({child.Defines.FullName})");
        }

        builder.AppendLine(")]");
        return builder.ToString();
    }

    /// <remarks><paramref name="root"/> is distinct from the default parent. The default parent has
    /// already been applied to the <see cref="Rule"/>. <paramref name="root"/> is used to apply
    /// a base interface outside of the default parent.</remarks>
    public static string BaseTypes(Rule rule, string? root = null)
    {
        var parents = rule.Parents.OfType<ExternalSymbol>().Select(p => p.FullName)
                          .Concat(rule.BaseRules.Select(r => TypeName(r.Defines)))
                          .ToFixedList();

        bool anyParents = parents.Any();
        if (!anyParents && root is null) return "";

        if (root is not null && !anyParents)
            parents = parents.Append(root).ToFixedList();

        return " : " + string.Join(", ", parents);
    }

    public static string TypeName(Symbol symbol)
        => symbol.FullName;

    public static string QualifiedTypeName(Symbol symbol)
        => symbol switch
        {
            InternalSymbol sym => $"{sym.ReferencedRule.Language.Name}.{symbol.FullName}",
            ExternalSymbol sym => sym.FullName,
            _ => throw ExhaustiveMatch.Failed(symbol)
        };

    public static string Type(Type type) => Type(type, TypeName);

    public static string QualifiedType(Type type) => Type(type, QualifiedTypeName);

    private static string Type(Type type, Func<Symbol, string> emitSymbol)
        => type switch
        {
            SymbolType t => emitSymbol(t.Symbol),
            ListType t => $"IFixedList<{Type(t.ElementType, emitSymbol)}>",
            SetType t => $"IFixedSet<{Type(t.ElementType, emitSymbol)}>",
            OptionalType t => $"{Type(t.UnderlyingType, emitSymbol)}?",
            VoidType _ => "void",
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static string PropertyIsNew(Property property)
        => property.IsNewDefinition ? "new " : "";

    public static string ClassName(Rule rule) => ClassName(rule.Language, rule.Defines);

    public static string ClassName(Language language, Symbol? symbol)
        => symbol switch
        {
            null => $"{language.Grammar.Suffix}Node",
            InternalSymbol sym => $"{sym.FullName}Node",
            ExternalSymbol sym => sym.FullName,
            _ => throw ExhaustiveMatch.Failed(symbol)
        };

    public static string PropertyParameters(Rule rule)
        => string.Join(", ", rule.AllProperties.Select(p => $"{ParameterType(p.Type)} {p.Name.ToCamelCase()}"));

    private static string ParameterType(Type type)
        => type switch
        {
            SymbolType t => TypeName(t.Symbol),
            ListType t => $"IEnumerable<{ParameterType(t.ElementType)}>",
            SetType t => $"IEnumerable<{ParameterType(t.ElementType)}>",
            OptionalType t => $"{ParameterType(t.UnderlyingType)}?",
            VoidType _ => throw new NotSupportedException("Void type is not supported as a parameter type."),
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static string PropertyClassParameters(Rule rule)
        => string.Join(", ", rule.AllProperties.Select(p => $"{Type(p.Type)} {p.Name.ToCamelCase()}"));

    public static string PropertyArguments(Rule rule)
    {
        return string.Join(", ", rule.AllProperties.Select(ToArgument));

        static string ToArgument(Property p)
        {
            var parameterName = p.Name.ToCamelCase();
            return p.Type switch
            {
                SymbolType t => parameterName,
                ListType t => $"{parameterName}.ToFixedList()",
                SetType t => $"{parameterName}.ToFixedSet()",
                OptionalType t => parameterName,
                _ => throw ExhaustiveMatch.Failed(p.Type)
            };
        }
    }

    public static string SmartClassName(InternalSymbol symbol)
    {
        var correctLanguage = symbol.ReferencedRule.DefinedInLanguage;
        return ClassName(correctLanguage, symbol);
    }

    public static string Arguments(IEnumerable<Parameter> parameters)
        => string.Join(", ", parameters.Select(ParameterName));

    private static string ParameterName(Parameter parameter)
        => parameter.Name;
}
