using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types.Type;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(Rule rule, string indent = "")
    {
        var children = rule.ChildRules;
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
                          .Concat(rule.ParentRules.Select(r => TypeName(r.Defines)))
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

    public static string PassTypeName(Pass pass, Symbol symbol)
    {
        if (symbol == Symbol.Void) return "Void";
        switch (symbol)
        {
            default:
                throw ExhaustiveMatch.Failed(symbol);
            case ExternalSymbol _:
                return symbol.FullName;
            case InternalSymbol sym:
                var language = sym.ReferencedRule.Language;
                var languageName = language switch
                {
                    _ when language == pass.FromLanguage => "From",
                    _ when language == pass.ToLanguage => "To",
                    _ => throw new FormatException($"Invalid symbol for pass type name '{symbol}'")
                };
                return $"{languageName}.{symbol.FullName}";
        }
    }

    public static string Type(Type type) => Type(type, TypeName);

    public static string QualifiedType(Type type) => Type(type, QualifiedTypeName);

    public static string PassParameterType(Pass pass, Type type)
        => type switch
        {
            SymbolType t => PassTypeName(pass, t.Symbol),
            ListType t => $"IEnumerable<{PassParameterType(pass, t.ElementType)}>",
            SetType t => $"IEnumerable<{PassParameterType(pass, t.ElementType)}>",
            OptionalType t => $"{PassParameterType(pass, t.UnderlyingType)}?",
            VoidType _ => throw new NotSupportedException("Void type is not supported as a parameter type."),
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static string PassReturnType(Pass pass, Type type)
        => Type(type, s => PassTypeName(pass, s));

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

    public static string UsingAlias(string alias, Language? language)
    {
        if (language is null) return "";
        return $"using {alias} = {language.Grammar.Namespace}.{language.Name};\r\n";
    }

    public static string TransformInterfaceTypeParameters(Pass pass)
        => string.Join(", ", TransformInterfaceTypeParametersInternal(pass));

    private static IEnumerable<string> TransformInterfaceTypeParametersInternal(Pass pass)
    {
        yield return FromEntryType(pass);

        yield return ContextType(pass.FromContext);

        yield return ToEntryType(pass);

        yield return ContextType(pass.ToContext);
    }

    private static string FromEntryType(Pass pass)
        => EntryType(pass.FromLanguage, pass.From, "From");

    private static string ToEntryType(Pass pass)
        => EntryType(pass.ToLanguage, pass.To, "To");

    private static string EntryType(Language? language, SymbolNode? symbol, string languageAlias)
    {
        if (language is not null)
            return $"{languageAlias}.{TypeName(language.Entry)}";
        return symbol is not null ? symbol.Text : "Void";
    }

    private static string ContextType(Symbol? fromContext)
        => fromContext is not null ? fromContext.FullName : "Void";

    public static string FullRunParameters(Pass pass)
        => Parameters(pass, pass.FullRunParameters);

    public static string RunParameters(Pass pass)
        => Parameters(pass, pass.RunParameters);

    public static string Parameters(Transform transform)
        => Parameters(transform.Pass, transform.From);

    public static string Arguments(IEnumerable<Parameter> parameters)
        => string.Join(", ", parameters.Select(ParameterName));

    private static string ParameterName(Parameter parameter)
        => parameter.Name;

    public static string Parameters(Pass pass, IEnumerable<Parameter> parameters)
        => string.Join(", ", parameters.Select(p => Parameter(pass, p)));

    public static string Parameter(Pass pass, Parameter parameter)
        => $"{PassParameterType(pass, parameter.Type)} {parameter.Name}";

    public static string FullRunReturnType(Pass pass)
        => PassReturnType(pass, pass.FullRunReturnValues);

    public static string RunReturnType(Pass pass)
        => PassReturnType(pass, pass.RunReturnValues);

    public static string RunReturnNames(Pass pass)
        => ReturnNames(pass.RunReturnValues);

    public static string ReturnNames(IFixedList<Parameter> returnValues)
    {
        return returnValues.Count switch
        {
            0 => "",
            1 => ParameterName(returnValues[0]),
            _ => $"({string.Join(", ", returnValues.Select(ParameterName))})"
        };
    }

    public static string ReturnType(Transform transform)
        => PassReturnType(transform.Pass, transform.To);

    public static string PassReturnType(Pass pass, IFixedList<Parameter> returnValues)
    {
        return returnValues.Count switch
        {
            0 => "void",
            1 => PassReturnType(pass, returnValues[0].Type),
            _ => $"({string.Join(", ", returnValues.Select(p => PassReturnType(pass, p.Type)))})"
        };
    }

    public static string RunForward(Pass pass)
    {
        var result = $"Run({Arguments(pass.RunParameters)})";
        if (pass.ToContext is null) return $"({result}, default)";
        if (pass.To is null) return $"(default, {result})";
        return result;
    }

    public static string ContextParameterName(Pass pass)
        => pass.FromContextParameter is not null ? ParameterName(pass.FromContextParameter) : "";

    public static string EntryResult(Pass pass)
        => Result(pass.EntryTransform.To);

    private static string Result(IFixedList<Parameter> returnValues)
    {
        return returnValues.Count switch
        {
            0 => "",
            1 => $"var {returnValues[0].Name} = ",
            _ => $"var ({Arguments(returnValues)} = ",
        };
    }

    public static string MethodName(Pass pass)
        => pass.To is not null ? "Transform" : "Analyze";

    public static string EntryParameterNames(Pass pass)
        => Arguments(pass.EntryTransform.From);

    public static string AccessModifier(Transform transform)
        => AccessModifier(transform.To);

    private static string AccessModifier(IEnumerable<Parameter> returnValues)
        => returnValues.Any() ? "private " : "";

    #region StartRun()
    public static string StartRunAccessModifier(Pass pass) => AccessModifier(StartRunReturnValues(pass));

    public static string StartRunReturnType(Pass pass)
        => PassReturnType(pass, StartRunReturnValues(pass).ToFixedList());

    private static IEnumerable<Parameter> StartRunReturnValues(Pass pass)
    {
        if (pass.From is null) return pass.EntryTransform.From;
        return pass.EntryTransform.From.Skip(1);
    }

    public static string StartRunResult(Pass pass)
    {
        var skip = pass.From is null ? 0 : 1;
        return Result(pass.EntryTransform.From.Skip(skip).ToFixedList());
    }
    #endregion

    #region EndRun()
    public static string EndRunAccessModifier(Pass pass)
        => EndRunReturnValues(pass).Any() ? "private " : "";

    public static string EndRunParameters(Pass pass)
        => Parameters(pass, pass.EntryTransform.To);

    public static string EndRunReturnType(Pass pass)
        => PassReturnType(pass, EndRunReturnValues(pass).ToFixedList());

    private static IEnumerable<Parameter> EndRunReturnValues(Pass pass)
    {
        if (pass.ToContextParameter is not null)
            yield return pass.ToContextParameter;
    }

    public static string EndRunArguments(Pass pass)
        => Arguments(pass.EntryTransform.To);

    public static string EndRunResult(Pass pass)
        => Result(pass.ToContextParameter.YieldValue().ToFixedList());
    #endregion

    public static string TransformMethodBody(Transform transform)
    {
        var fromType = transform.From[0].Type;
        var toType = transform.To[0].Type;
        if (fromType is CollectionType fromCollectionType && toType is CollectionType toCollectionType)
        {
            var resultCollection = toCollectionType switch
            {
                ListType _ => "FixedList",
                SetType _ => "FixedSet",
                _ => throw ExhaustiveMatch.Failed(toCollectionType)
            };
            var calledParameters = transform.From.Skip(1).Prepend(Model.Parameter.Create(fromCollectionType.ElementType, "from")).ToFixedList();
            var calledTransform = transform.Pass.Transforms.SingleOrDefault(t => t.From.SequenceEqual(calledParameters));
            var calledTransformReturnsCollection = calledTransform?.To[0].Type is CollectionType;
            var selectMethod = calledTransformReturnsCollection ? "SelectMany" : "Select";
            var parameters = transform.From.Skip(1).Select(ParameterName).Prepend("f").ToFixedList();
            var parameterNames = string.Join(", ", parameters);
            return $"{ParameterName(transform.From[0])}.{selectMethod}(f => {MethodName(transform.Pass)}({parameterNames})).To{resultCollection}()";
        }

        return $"Create({Arguments(transform.From)})";
    }

    public static string ModifiedParameters(Pass pass, Rule rule)
    {
        var extendsRule = rule.ExtendsRule!;
        var modifiedProperties = rule.AllProperties
                                     .Where(CouldBeModified)
                                     .Except(extendsRule.AllProperties, Property.NameAndTypeComparer);
        var fromType = new SymbolType(extendsRule.Defines);
        var parameters = new List<Parameter> { Model.Parameter.Create(fromType, "from") };
        parameters.AddRange(modifiedProperties.Select(p => Model.Parameter.Create(p.Type, p.Name.ToCamelCase())));
        return Parameters(pass, parameters);
    }

    private static bool CouldBeModified(Property property)
        => property.Type.UnderlyingSymbol is
            InternalSymbol { ReferencedRule.DescendantsModified: true } or ExternalSymbol;

    public static string SimpleCreateParameters(Rule rule)
    {
        var extendsRule = rule.ExtendsRule!;
        var oldProperties = new HashSet<Property>(Property.NameAndTypeComparer);
        oldProperties.AddRange(extendsRule.AllProperties);
        var parameters = new List<string>();
        foreach (var property in rule.AllProperties)
            parameters.Add(oldProperties.Contains(property) || !CouldBeModified(property)
                ? $"from.{property.Name}"
                : property.Name.ToCamelCase());
        return string.Join(", ", parameters);
    }
}
