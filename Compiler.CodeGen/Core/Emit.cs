using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
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

    public static string TypeName(Type type)
        => TypeName(type.Symbol);

    public static string TypeName(Symbol symbol)
        => symbol.FullName;

    public static string QualifiedTypeName(Type type)
        => QualifiedTypeName(type.Symbol);

    public static string QualifiedTypeName(Symbol symbol)
        => symbol switch
        {
            InternalSymbol sym => $"{sym.ReferencedRule.Language.Name}.{symbol.FullName}",
            ExternalSymbol sym => sym.FullName,
            _ => throw ExhaustiveMatch.Failed(symbol)
        };

    public static string Type(Type type)
    {
        var name = TypeName(type);
        return TypeDecorations(type, name);
    }

    public static string QualifiedType(Type type)
    {
        var name = QualifiedTypeName(type);
        return TypeDecorations(type, name);
    }

    private static string TypeDecorations(Type type, string name)
    {
        switch (type.CollectionKind)
        {
            default:
                throw ExhaustiveMatch.Failed(type.CollectionKind);
            case CollectionKind.None:
                // Nothing
                break;
            case CollectionKind.List:
                name = $"{CollectionType(type)}<{name}>";
                break;
            case CollectionKind.Set:
                name = $"{CollectionType(type)}<{name}>";
                break;
        }

        if (type.IsOptional) name += "?";
        return name;
    }

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
    {
        var name = TypeName(type);
        return ParameterTypeDecorations(type, name);
    }

    private static string ParameterTypeDecorations(Type type, string name)
    {
        if (type.CollectionKind != CollectionKind.None) name = $"IEnumerable<{name}>";
        if (type.IsOptional) name += "?";
        return name;
    }

    public static string PropertyClassParameters(Rule rule)
        => string.Join(", ", rule.AllProperties.Select(p => $"{Type(p.Type)} {p.Name.ToCamelCase()}"));

    public static string PropertyArguments(Rule rule)
    {
        return string.Join(", ", rule.AllProperties.Select(ToArgument));
        static string ToArgument(Property p)
        {
            var collectionType = CollectionType(p.Type);
            if (collectionType is not null)
                return $"{p.Name.ToCamelCase()}.To{collectionType.WithoutInterfacePrefix()}()";
            return p.Name.ToCamelCase();
        }
    }

    private static string? CollectionType(Type type)
    {
        return type.CollectionKind switch
        {
            CollectionKind.None => null,
            CollectionKind.List => "IFixedList",
            CollectionKind.Set => "IFixedSet",
            _ => throw ExhaustiveMatch.Failed(type.CollectionKind)
        };
    }

    private static string WithoutInterfacePrefix(this string name)
    {
        if (name.StartsWith("I")) return name[1..];
        return name;
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

    public static string EndRunParameterNames(Pass pass)
        => ParameterNames(pass.EntryTransform.To);

    public static string Parameters(Transform transform)
        => Parameters(transform.Pass, transform.From);

    public static string ParameterNames(IEnumerable<Parameter> parameters)
        => string.Join(", ", parameters.Select(ParameterName));

    private static string ParameterName(Parameter parameter)
        => parameter == Model.Parameter.Void ? "" : parameter.Name;

    public static string Parameters(Pass pass, IEnumerable<Parameter> parameters)
        => string.Join(", ", parameters.Select(p => Parameter(pass, p)));

    public static string Parameter(Pass pass, Parameter parameter)
        => $"{PassParameterType(pass, parameter.Type)} {parameter.Name}";

    public static string PassParameterType(Pass pass, Type type)
    {
        var name = PassTypeName(pass, type);
        return ParameterTypeDecorations(type, name);
    }

    public static string PassTypeName(Pass pass, Type type)
        => PassTypeName(pass, type.Symbol);

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

    public static string FullRunReturnType(Pass pass)
        => PassReturnType(pass, pass.FullRunReturn);

    public static string RunReturnType(Pass pass)
        => PassReturnType(pass, pass.RunReturn);

    public static string RunReturnNames(Pass pass)
        => ReturnNames(pass.RunReturn);

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

    public static string PassReturnType(Pass pass, Type type)
    {
        var name = PassTypeName(pass, type);
        return TypeDecorations(type, name);
    }

    public static string RunForward(Pass pass)
    {
        var result = $"Run({ParameterNames(pass.RunParameters)})";
        if (pass.ToContext is null) return $"({result}, default)";
        if (pass.To is null) return $"(default, {result})";
        return result;
    }

    public static string ContextParameterName(Pass pass)
        => ParameterName(pass.FromContextParameter);

    public static string EntryResult(Pass pass)
        => Result(pass.EntryTransform.To);

    private static string Result(IFixedList<Parameter> returnValues)
    {
        return returnValues.Count switch
        {
            0 => "",
            1 => $"var {returnValues[0].Name} = ",
            _ => $"var ({ParameterNames(returnValues)} = ",
        };
    }

    public static string MethodName(Pass pass)
        => pass.To is not null ? "Transform" : "Analyze";

    public static string EntryParameterNames(Pass pass)
        => ParameterNames(pass.EntryTransform.From);

    public static string EndRunResult(Pass pass)
        => pass.ToContextParameter.Type == Model.Types.Type.Void ? "" : $"var {pass.ToContextParameter.Name} = ";

    public static string StartRunAccessModifier(Pass pass)
        => AccessModifier(StartRunReturnValues(pass));

    public static string AccessModifier(Transform transform)
        => AccessModifier(transform.To);

    private static string AccessModifier(IEnumerable<Parameter> returnValues)
        => returnValues.Any() ? "private " : "";

    public static string StartRunReturnType(Pass pass)
        => PassReturnType(pass, StartRunReturnValues(pass).ToFixedList());

    private static IEnumerable<Parameter> StartRunReturnValues(Pass pass)
    {
        if (pass.From is null) return pass.EntryTransform.From;
        return pass.EntryTransform.From.Skip(1);
    }

    public static string EndRunAccessModifier(Pass pass)
        => EndRunReturnValues(pass).Any() ? "private " : "";

    public static string EndRunReturnType(Pass pass)
        => PassReturnType(pass, EndRunReturnValues(pass).ToFixedList());

    private static IEnumerable<Parameter> EndRunReturnValues(Pass pass)
    {
        if (pass.ToContextParameter.Type != Model.Types.Type.Void)
            yield return pass.ToContextParameter;
    }

    public static string EndRunParameters(Pass pass)
        => Parameters(pass, pass.EntryTransform.To);

    public static string TransformMethodBody(Transform transform)
    {
        var fromType = transform.From[0].Type;
        var toType = transform.To[0].Type;
        if (fromType.IsCollection && toType.IsCollection)
        {
            var resultCollection = toType.CollectionKind switch
            {
                CollectionKind.List => "FixedList",
                CollectionKind.Set => "FixedSet",
                CollectionKind.None => throw new UnreachableException(),
                _ => throw ExhaustiveMatch.Failed(toType.CollectionKind)
            };
            var calledParameters = transform.From.Skip(1).Prepend(Model.Parameter.Create(fromType.UnderlyingType, "from")).ToFixedList();
            var calledTransform = transform.Pass.Transforms.SingleOrDefault(t => t.From.SequenceEqual(calledParameters));
            var calledTransformReturnsCollection = calledTransform?.To[0].Type.IsCollection ?? false;
            var selectMethod = calledTransformReturnsCollection ? "SelectMany" : "Select";
            var parameters = transform.From.Skip(1).Select(ParameterName).Prepend("f").ToFixedList();
            var parameterNames = string.Join(", ", parameters);
            return $"{ParameterName(transform.From[0])}.{selectMethod}(f => {MethodName(transform.Pass)}({parameterNames})).To{resultCollection}()";
        }

        return $"Create({ParameterNames(transform.From)})";
    }

    public static string ModifiedParameters(Pass pass, Rule rule)
    {
        var extendsRule = rule.ExtendsRule!;
        var modifiedProperties = rule.AllProperties
                                     .Where(CouldBeModified)
                                     .Except(extendsRule.AllProperties, Property.NameAndTypeComparer);
        var fromType = Model.Types.Type.Create(extendsRule.Defines);
        var parameters = new List<Parameter> { Model.Parameter.Create(fromType, "from") };
        parameters.AddRange(modifiedProperties.Select(p => Model.Parameter.Create(p.Type, p.Name.ToCamelCase())));
        return Parameters(pass, parameters);
    }

    private static bool CouldBeModified(Property property)
        => property.Type.Symbol is InternalSymbol { ReferencedRule.DescendantsModified: true } or ExternalSymbol;

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
