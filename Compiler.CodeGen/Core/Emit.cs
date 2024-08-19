using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(TreeNodeModel node, string indent = "")
    {
        var children = node.ChildNodes;
        if (!children.Any()) return $"// [Closed(typeof({node.Defines.ClassName}))]{Environment.NewLine}";
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

    public static string BaseTypes(TreeNodeModel node)
    {
        var supertypes = node.Supertypes.OfType<ExternalSymbol>().Select(p => p.FullName)
                          .Concat(node.SupertypeNodes.Select(r => TypeName(r.Defines)))
                          .ToFixedList();

        if (supertypes.IsEmpty) return "";

        return " : " + string.Join(", ", supertypes);
    }

    public static string TypeName(Symbol symbol)
        => symbol.FullName;

    public static string Type(TypeModel type) => Type(type, TypeName);

    private static string Type(TypeModel type, Func<Symbol, string> emitSymbol)
        => type switch
        {
            SymbolType t => emitSymbol(t.Symbol),
            ListType t => $"IFixedList<{Type(t.ElementType, emitSymbol)}>",
            SetType t => $"IFixedSet<{Type(t.ElementType, emitSymbol)}>",
            OptionalType t => $"{Type(t.UnderlyingType, emitSymbol)}?",
            _ => throw ExhaustiveMatch.Failed(type)
        };

    public static string ClassName(InternalSymbol symbol) => symbol.ClassName;

    public static string BaseClassName(TreeModel tree)
    {
        if (tree.RootSupertype is null) return $"{tree.ClassPrefix}{tree.ClassSuffix}";
        return tree.RootSupertype.ClassName;
    }

    public static string BaseClass(TreeModel tree)
        => tree.SimplifiedTree ? "" : $"{BaseClassName(tree)}, ";

    public static string IsNew(AttributeModel attribute)
        => attribute.IsNewDefinition ? "new " : "";

    public static string ParameterName(AttributeModel attribute)
        => attribute.Name.ToCamelCase();

    public static string Override(SynthesizedAttributeEquationModel equation)
        => ObjectMembers.Contains(equation.Name) ? "override " : "";

    private static readonly IFixedSet<string> ObjectMembers = new HashSet<string>()
    {
        "ToString",
        "GetHashCode",
        "Equals",
    }.ToFixedSet();

    public static string Body(SynthesizedAttributeModel attribute)
    {
        if (attribute.DefaultExpression is not null)
            return $"{Environment.NewLine}        => {attribute.DefaultExpression};";
        if (!string.IsNullOrEmpty(attribute.Parameters))
            return ";";

        return " { get; }";
    }

    public static string Body(SynthesizedAttributeEquationModel equation)
    {
        switch (equation.Strategy)
        {
            default:
                throw ExhaustiveMatch.Failed(equation.Strategy);
            case EvaluationStrategy.Eager:
                throw new NotSupportedException("Eager equations must be inside constructor.");
            case EvaluationStrategy.Lazy:
            {
                var builder = new StringBuilder();
                builder.AppendLine();
                var value = equation.Name.ToCamelCase();
                var cached = equation.Name.ToCamelCase() + "Cached";
                var notNull = equation.Type is OptionalType ? "" : "!";
                builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}{notNull}");
                builder.AppendLine($"            : this.Synthetic(ref {cached}, ref {value},");
                builder.Append("                ");
                AppendQualifiedEquationMethod(equation, builder);
                builder.AppendLine(");");
                builder.AppendLine($"    private {Type(equation.Type.ToNonOptional())}? {value};");
                builder.Append($"    private bool {cached};");
                return builder.ToString();
            }
            case EvaluationStrategy.Computed:
            {
                var builder = new StringBuilder();
                builder.AppendLine();
                builder.Append("        => ");
                if (equation.Expression is not null)
                    builder.Append(equation.Expression);
                else
                {
                    AppendQualifiedEquationMethod(equation, builder);
                    builder.Append("(this)");
                }

                builder.Append(';');
                return builder.ToString();
            }
        }
    }

    private static void AppendQualifiedEquationMethod(SynthesizedAttributeEquationModel equation, StringBuilder builder)
    {
        builder.Append(equation.Aspect.Name);
        builder.Append('.');
        AppendEquationMethod(equation, builder);
    }

    public static string EquationMethod(SynthesizedAttributeEquationModel equation)
    {
        var builder = new StringBuilder();
        AppendEquationMethod(equation, builder);
        return builder.ToString();
    }

    private static void AppendEquationMethod(SynthesizedAttributeEquationModel equation, StringBuilder builder)
    {
        builder.Append(equation.NodeSymbol);
        builder.Append('_');
        builder.Append(equation.Name);
    }

    public static string EagerBody(SynthesizedAttributeEquationModel equation)
    {
        Requires.That(equation.Strategy == EvaluationStrategy.Eager, nameof(equation), "Must be an eager equation.");
        return equation.Expression ?? $"{equation.Aspect.Name}.{equation.NodeSymbol}_{equation.Name}(this)";
    }

    public static string Parameters(InheritedAttributeModel attribute)
        => attribute.IsMethod ? "()" : "";

    public static string Body(InheritedAttributeModel attribute)
    {
        switch (attribute.Strategy)
        {
            default:
                throw ExhaustiveMatch.Failed(attribute.Strategy);
            case EvaluationStrategy.Eager:
                throw new UnreachableException("Inherited equations cannot be eager.");
            case EvaluationStrategy.Lazy:
            {
                var builder = new StringBuilder();
                builder.AppendLine();
                var value = attribute.Name.ToCamelCase();
                var cached = attribute.Name.ToCamelCase() + "Cached";
                var notNull = attribute.Type is OptionalType ? "" : "!";
                var supertype = attribute.AttributeSupertype.Type;
                var castRequired = attribute.Type != supertype;
                var castStart = castRequired ? $"(ctx) => ({Type(attribute.Type)})" : "";
                var castEnd = castRequired ? "(ctx)" : "";
                builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}{notNull}");
                builder.AppendLine($"            : this.Inherited(ref {cached}, ref {value},");
                builder.AppendLine($"                {castStart}Inherited{attribute.Name}{castEnd});");
                builder.AppendLine($"    private {Type(attribute.Type.ToNonOptional())}? {value};");
                builder.Append($"    private bool {cached};");
                return builder.ToString();
            }
            case EvaluationStrategy.Computed:
                return $"        => Inherited{attribute.Name}(GrammarAttribute.CurrentInheritanceContext());";
        }
    }

    public static string Selector(InheritedAttributeEquationModel equation)
        => Selector(equation.Selector);

    private static string Selector(SelectorModel selector)
        => selector switch
        {
            AllChildrenSelectorModel _ => SelectorIf("ReferenceEquals(child, descendant)"),
            DescendantsSelectorModel _ => "", // always
            // TODO use Current child for all of these
            ChildSelectorModel s => SelectorIf($"ReferenceEquals(descendant, {s.Child})"),
            ChildAtIndexSelectorModel s => SelectorIf($"ReferenceEquals(descendant, {s.Child}[{s.Index}])"),
            ChildAtVariableSelectorModel s => SelectorIf($"{s.Child}.IndexOf(descendant) is {{}} {s.Variable}"),
            _ => throw ExhaustiveMatch.Failed(selector)
        };

    private static string SelectorIf(string condition)
        => "if(" + condition + $"){Environment.NewLine}            ";

    public static string Body(InheritedAttributeEquationModel equation)
    {
        if (equation.Expression is not null)
            return equation.Expression;

        var parameters = equation.Selector is ChildAtVariableSelectorModel s ? $"(this, {s.Variable})" : "(this)";
        return $"{equation.Aspect.Name}." + EquationMethod(equation) + parameters;
    }

    public static string EquationMethod(InheritedAttributeEquationModel equation)
        => $"{equation.NodeSymbol}_" + equation.Selector switch
        {
            AllChildrenSelectorModel _ => "Children",
            DescendantsSelectorModel _ => "Descendants", // always
            ChildSelectorModel s => s.Child,
            ChildAtIndexSelectorModel s => $"{s.Child}_{s.Index}",
            ChildAtVariableSelectorModel s => s.Child,
            _ => throw ExhaustiveMatch.Failed(equation.Selector)
        } + $"_{equation.Name}";

    public static string EquationMethodExtraParams(InheritedAttributeEquationModel equation)
    {
        if (equation.Selector is ChildAtVariableSelectorModel s)
            return $", int {s.Variable}";
        return "";
    }
}
