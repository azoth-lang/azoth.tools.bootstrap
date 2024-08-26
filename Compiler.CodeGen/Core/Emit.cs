using System;
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
            SymbolTypeModel t => emitSymbol(t.Symbol),
            ListTypeModel t => $"IFixedList<{Type(t.ElementType, emitSymbol)}>",
            SetTypeModel t => $"IFixedSet<{Type(t.ElementType, emitSymbol)}>",
            OptionalTypeModel t => $"{Type(t.UnderlyingType, emitSymbol)}?",
            EnumerableTypeModel t => $"IEnumerable<{Type(t.ElementType, emitSymbol)}>",
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

    #region Attributes
    public static string IsNew(AttributeModel attribute)
        => attribute.IsNewDefinition ? "new " : "";

    public static string VariableName(AttributeModel attribute)
        => attribute.Name.ToCamelCase();

    public static string RewritableBackingType(PropertyModel property)
    {
        switch (property.Type)
        {
            case SetTypeModel:
                throw new NotImplementedException($"Rewritable backing type not yet implemented for {property.Type}.");
            case CollectionTypeModel collectionType:
                var finalElementType = ((CollectionTypeModel)property.FinalType).ElementType.ToNonOptional();
                return property.IsTemp
                    ? $"IRewritableChildList<{Type(collectionType.ElementType)}, {Type(finalElementType)}>"
                    : $"IRewritableChildList<{Type(property.Type)}>";
            default:
                throw new NotImplementedException($"Rewritable backing type not yet implemented for {property.Type}.");
        }
    }

    public static string ChildAttach(PropertyModel property)
    {
        switch (property.Type)
        {
            case SetTypeModel:
                throw new NotImplementedException($"{nameof(ChildAttach)} not yet implemented for {property.Type}.");
            case CollectionTypeModel:

                var childListClass = "ChildList";
                if (property.IsTemp)
                {
                    var finalElementType = ((CollectionTypeModel)property.FinalType).ElementType.ToNonOptional();
                    childListClass += $"<{Type(finalElementType)}>";
                }
                return $"{childListClass}.Create(this, nameof({property.Name}), {VariableName(property)})";
            default:
                throw new NotImplementedException($"{nameof(ChildAttach)} not yet implemented for {property.Type}.");
        }
    }

    public static string Parameters(AttributeModel attribute)
    {
        if (attribute is IntertypeMethodAttributeModel method)
            return $"({method.Parameters})";
        return attribute.IsMethod ? "()" : "";
    }

    public static string ParametersAndBody(AttributeModel attribute)
        => attribute switch
        {
            PropertyModel _ => " { get; }",
            SynthesizedAttributeModel a => ParametersAndBody(a),
            InheritedAttributeModel a => a.IsMethod ? "();" : " { get; }",
            PreviousAttributeModel a => a.IsMethod ? "();" : " { get; }",
            IntertypeMethodAttributeModel a => ParametersAndBody(a),
            _ => throw ExhaustiveMatch.Failed(attribute)
        };

    private static string ParametersAndBody(SynthesizedAttributeModel attribute)
    {
        if (attribute.Strategy == EvaluationStrategy.Computed && !attribute.IsObjectMember())
        {
            var expression = attribute.DefaultExpression ?? ExpressionFor(attribute);
            if (expression is not null)
                return $"{Parameters(attribute)}{Environment.NewLine}        => {expression};";
        }
        return attribute.IsMethod ? "();" : " { get; }";
    }

    private static string ParametersAndBody(IntertypeMethodAttributeModel attribute)
    {
        if (!attribute.IsObjectMember())
        {
            var expression = attribute.DefaultExpression ?? ExpressionFor(attribute);
            if (expression is not null)
                return $"{Parameters(attribute)}{Environment.NewLine}        => {expression};";
        }
        return $"{Parameters(attribute)};";
    }

    private static string? ExpressionFor(AttributeModel attribute)
    {
        if (attribute.Node.EquationFor(attribute) is not { } equation)
            return null;
        if (equation.Expression is not null)
            return equation.Expression;
        var builder = new StringBuilder();
        AppendQualifiedEquationMethod(equation, builder);
        builder.Append("(this)");
        return builder.ToString();
    }

    public static string Body(ContextAttributeModel attribute)
    {
        switch (attribute.Strategy)
        {
            default:
                throw ExhaustiveMatch.Failed(attribute.Strategy);
            case EvaluationStrategy.Eager:
                throw new UnreachableException($"{attribute.MethodPrefix} equations cannot be eager.");
            case EvaluationStrategy.Lazy:
            {
                var builder = new StringBuilder();
                builder.AppendLine();
                var value = attribute.Name.ToCamelCase();
                var cached = attribute.Name.ToCamelCase() + "Cached";
                var isValueType = attribute.Type.IsValueType;
                var notNull = attribute.Type is OptionalTypeModel || isValueType ? "" : "!";
                // Remove any optional type that might be on it and then ensure there is one optional type
                var fieldType = !isValueType ? attribute.Type.ToOptional() : attribute.Type;
                var syncLock = isValueType ? " ref syncLock," : "";
                var supertype = attribute.AttributeSupertype.Type;
                var castRequired = attribute.Type != supertype;
                var castStart = castRequired ? $"(ctx) => ({Type(attribute.Type)})" : "";
                var castEnd = castRequired ? "(ctx)" : "";
                builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}{notNull}");
                builder.AppendLine($"            : this.Inherited(ref {cached}, ref {value},{syncLock}");
                builder.AppendLine($"                {castStart}{attribute.MethodPrefix}_{attribute.Name}{castEnd});");
                builder.AppendLine($"    private {Type(fieldType)} {value};");
                builder.Append($"    private bool {cached};");
                return builder.ToString();
            }
            case EvaluationStrategy.Computed:
            {
                var supertype = attribute.AttributeSupertype.Type;
                var castRequired = attribute.Type != supertype;
                var cast = castRequired ? $"({Type(attribute.Type)})" : "";
                return $"{Environment.NewLine}        "
                       + $"=> {cast}{attribute.MethodPrefix}_{attribute.Name}(GrammarAttribute.CurrentInheritanceContext());";
            }
        }
    }
    #endregion

    #region Equations
    public static string Parameters(EquationModel equation)
    {
        if (equation is IntertypeMethodEquationModel method)
            return $"({method.Parameters})";
        return equation.IsMethod ? "()" : "";
    }

    public static string Override(SynthesizedAttributeEquationModel equation)
        => equation.IsObjectMember() ? "override " : "";

    public static string ParametersAndBody(SubtreeEquationModel equation)
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
                builder.Append(Parameters(equation));
                builder.AppendLine();
                var value = equation.Name.ToCamelCase();
                var cached = equation.Name.ToCamelCase() + "Cached";
                var isValueType = equation.Type.IsValueType;
                var notNull = equation.Type is OptionalTypeModel || isValueType ? "" : "!";
                // Remove any optional type that might be on it and then ensure there is one optional type
                var fieldType = !isValueType ? equation.Type.ToOptional() : equation.Type;
                var syncLock = isValueType ? " ref syncLock," : "";
                builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}{notNull}");
                builder.AppendLine($"            : this.Synthetic(ref {cached}, ref {value},{syncLock}");
                builder.Append("                ");
                AppendQualifiedEquationMethod(equation, builder);
                builder.AppendLine(");");
                builder.AppendLine($"    private {Type(fieldType)} {value};");
                builder.Append($"    private bool {cached};");
                return builder.ToString();
            }
            case EvaluationStrategy.Computed:
            {
                // Case needed for object members like ToString()
                var builder = new StringBuilder();
                builder.Append(Parameters(equation));
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

    private static void AppendQualifiedEquationMethod(EquationModel equation, StringBuilder builder)
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

    private static void AppendEquationMethod(EquationModel equation, StringBuilder builder)
    {
        builder.Append(equation.NodeSymbol);
        builder.Append('_');
        builder.Append(equation.Name);
    }

    public static string EagerBody(SynthesizedAttributeEquationModel equation)
    {
        Requires.That(equation.Strategy == EvaluationStrategy.Eager, nameof(equation), "Must be an eager equation.");
        if (equation.Expression is not null)
            throw new NotImplementedException("Eager equations with expressions are not yet"
                                              + " implemented. They should emit a protected method to the interface and call that"
                                              + " from the constructor.");

        return $"{equation.Aspect.Name}.{equation.NodeSymbol}_{equation.Name}(this)";
    }

    public static string Selector(InheritedAttributeEquationModel equation)
        => Selector(equation.Selector);

    private static string Selector(SelectorModel selector)
        => selector switch
        {
            AllChildrenSelectorModel s
                => s.Broadcast ? "" // If broadcast, always matches, no condition needed
                    : SelectorIf("ReferenceEquals(child, descendant)"),
            // TODO use Current child for all of these
            ChildSelectorModel s => SelectorIf($"ReferenceEquals({ChildOrDescendant(s)}, Self.{s.Child})"),
            ChildAtIndexSelectorModel s => SelectorIf($"ReferenceEquals({ChildOrDescendant(s)}, Self.{s.Child}[{s.Index}])"),
            ChildAtVariableSelectorModel s => SelectorIf($"IndexOfNode(Self.{s.Child}, {ChildOrDescendant(s)}) is {{}} {s.Variable}"),
            ChildListSelectorModel s => SelectorIf($"ContainsNode(Self.{s.Child}, {ChildOrDescendant(s)})"),
            _ => throw ExhaustiveMatch.Failed(selector)
        };

    private static string SelectorIf(string condition)
        => "if (" + condition + $"){Environment.NewLine}            ";

    private static string ChildOrDescendant(SelectorModel selector)
        // Broadcast matches the child because it applies to all descendants under that child.
        // Whereas non-broadcast matches the descendant because the descendant must be a child.
        => selector.Broadcast ? "child" : "descendant";

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
            ChildSelectorModel s => s.Child,
            ChildAtIndexSelectorModel s => $"{s.Child}_{s.Index}",
            ChildAtVariableSelectorModel s => s.Child,
            ChildListSelectorModel s => s.Child,
            _ => throw ExhaustiveMatch.Failed(equation.Selector)
        } + (equation.Selector.Broadcast ? "_Broadcast" : "") + $"_{equation.Name}";

    public static string EquationMethodExtraParams(InheritedAttributeEquationModel equation)
    {
        if (equation.Selector is ChildAtVariableSelectorModel s)
            return $", int {s.Variable}";
        return "";
    }
    #endregion

    #region Rewrite Rules
    public static string RuleJoin(IFixedList<RewriteRuleModel> rules, RewriteRuleModel rule)
    {
        var isFirst = rules[0] == rule;
        return isFirst ? "=>" : "??";
    }

    public static string QualifiedRewriteRuleMethod(RewriteRuleModel rule)
        => $"{rule.Aspect.Name}.{RewriteRuleMethod(rule)}";

    public static string RewriteRuleMethod(RewriteRuleModel rule)
        => rule.Name is not null ? $"{rule.NodeSymbol}_Rewrite_{rule.Name}"
            : $"{rule.NodeSymbol}_Rewrite";
    #endregion
}
