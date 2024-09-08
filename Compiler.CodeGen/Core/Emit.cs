using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
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
        var supertypes = node.DeclaredSupertypes.OfType<ExternalSymbol>().Select(p => p.FullName)
                          .Concat(node.SupertypeNodes.Select(r => TypeName(r.Defines)))
                          .ToFixedList();

        if (supertypes.IsEmpty) return "";

        return " : " + string.Join(", ", supertypes);
    }

    public static string TypeName(Symbol symbol)
        => symbol.FullName;

    public static string Type(TypeModel type)
        => type switch
        {
            SymbolTypeModel t => TypeName(t.Symbol),
            ListTypeModel t => $"IFixedList<{Type(t.ElementType)}>",
            SetTypeModel t => $"IFixedSet<{Type(t.ElementType)}>",
            OptionalTypeModel t => $"{Type(t.UnderlyingType)}?",
            EnumerableTypeModel t => $"IEnumerable<{Type(t.ElementType)}>",
            _ => throw ExhaustiveMatch.Failed(type)
        };

    /// <summary>
    /// The type of a parameter to the constructor for a property of this type.
    /// </summary>
    public static string ParameterType(TypeModel type)
        => type switch
        {
            SymbolTypeModel t => TypeName(t.Symbol),
            CollectionTypeModel t => $"IEnumerable<{Type(t.ElementType)}>",
            OptionalTypeModel t => $"{ParameterType(t.UnderlyingType)}?",
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

    #region Members
    public static string ChildAttach(IMemberModel member)
    {
        switch (member.Type)
        {
            case SetTypeModel:
                return "ChildSet.Attach(this, ";
            case CollectionTypeModel:
                if (!member.MayHaveRewrites)
                    return "ChildList.Attach(this, ";

                var childListClass = "ChildList";
                if (member.IsTemp)
                {
                    var finalElementType = ((CollectionTypeModel)member.FinalType).ElementType.ToNonOptional();
                    childListClass += $"<{Type(finalElementType)}>";
                }
                return $"{childListClass}.Create(this, nameof({member.Name}), ";
            default:
                return member.MayHaveRewrites ? "Child.Create(this, " : "Child.Attach(this, ";
        }
    }
    #endregion

    #region Attributes
    public static string IsNew(AttributeModel attribute)
        => attribute.IsNewDefinition ? "new " : "";

    public static string VariableName(AttributeModel attribute)
        => attribute.Name.ToCamelCase();

    public static string ToCollection(PropertyModel property)
    {
        return property.Type switch
        {
            SetTypeModel _ => ".ToFixedSet()",
            ListTypeModel _ => ".ToFixedList()",
            EnumerableTypeModel _ => throw new NotImplementedException("Enumerable type not yet implemented."),
            OptionalTypeModel _ => "",
            SymbolTypeModel _ => "",
            _ => throw ExhaustiveMatch.Failed(property.Type)
        };
    }

    public static string FieldReference(PropertyModel property)
        => property.MayHaveRewrites ? $"this.{VariableName(property)}" : property.Name;

    public static string RewritableBackingType(PropertyModel property)
    {
        switch (property.Type)
        {
            case SetTypeModel _:
                throw new NotSupportedException($"Rewritable backing type not yet supported for {property.Type}.");
            case CollectionTypeModel collectionType:
                var finalElementType = ((CollectionTypeModel)property.FinalType).ElementType.ToNonOptional();
                return property.IsTemp
                    ? $"IRewritableChildList<{Type(collectionType.ElementType)}, {Type(finalElementType)}>"
                    : $"IRewritableChildList<{Type(property.Type)}>";
            case OptionalTypeModel _:
            case SymbolTypeModel _:
                return $"RewritableChild<{Type(property.Type)}>";
            default:
                throw ExhaustiveMatch.Failed(property.Type);
                //throw new NotImplementedException($"Rewritable backing type not yet implemented for {property.Type}.");
        }
    }

    public static string FinalValue(PropertyModel property)
    {
        if (!property.IsTemp)
            return VariableName(property);
        if (property.Type is CollectionTypeModel)
            return $"{VariableName(property)}.AsFinalType";
        return $"{property.TempName} as {Type(property.FinalType.ToNonOptional())}";
    }

    public static string CurrentValue(PropertyModel property)
    {
        if (property.Type is CollectionTypeModel)
            return $"{VariableName(property)}.Current";
        return $"{VariableName(property)}.UnsafeValue";
    }

    public static string Parameters(IEnumerable<PropertyModel> properties)
    {
        var parameters = string.Join($",{Environment.NewLine}        ",
            properties.Select(p => $"{ParameterType(p.Type)} {VariableName(p)}"));
        if (string.IsNullOrWhiteSpace(parameters))
            return "";
        return $"{Environment.NewLine}        " + parameters;
    }

    public static string Parameters(AttributeModel attribute)
    {
        if (attribute is IntertypeMethodAttributeModel method)
            return $"({method.Parameters})";
        return attribute.IsMethod ? "()" : "";
    }

    public static string Arguments(IEnumerable<PropertyModel> properties)
        => string.Join(", ", properties.Select(VariableName));

    public static string ParametersAndBody(AttributeModel attribute)
        => attribute switch
        {
            PropertyModel _ => " { get; }",
            AggregateAttributeModel _ => " { get; }",
            CollectionAttributeModel _ => " { get; }",
            SynthesizedAttributeModel a => ParametersAndBody(a),
            CircularAttributeModel _ => " { get; }",
            InheritedAttributeModel a => a.IsMethod ? "();" : " { get; }",
            PreviousAttributeModel a => a.IsMethod ? "();" : " { get; }",
            IntertypeMethodAttributeModel a => ParametersAndBody(a),
            // TODO Parent should only be allowed on final nodes
            ParentAttributeModel a => $" => ({Type(a.Type)})PeekParent()!;",
            PlaceholderModel _ => throw new NotSupportedException("Placeholder should not be emitted."),
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

    public static string Body(AttributeModel attribute)
        => attribute switch
        {
            ContextAttributeModel a => Body(a),
            AggregateAttributeModel a => Body(a),
            CollectionAttributeModel a => Body(a),
            _ => throw new NotSupportedException()
        };

    private static string Body(ContextAttributeModel attribute)
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
                var cached = value + "Cached";
                var isValueType = attribute.Type.IsValueType;
                var notNull = attribute.Type is OptionalTypeModel || isValueType ? "" : "!";
                // Remove any optional type that might be on it and then ensure there is one optional type
                var fieldType = !isValueType ? attribute.Type.ToOptional() : attribute.Type;
                var syncLock = isValueType ? " ref syncLock," : "";
                var supertype = attribute.AttributeFamily.Type;
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
                var supertype = attribute.AttributeFamily.Type;
                var castRequired = attribute.Type != supertype;
                var cast = castRequired ? $"({Type(attribute.Type)})" : "";
                return $"{Environment.NewLine}        "
                       + $"=> {cast}{attribute.MethodPrefix}_{attribute.Name}(GrammarAttribute.CurrentInheritanceContext());";
            }
        }
    }

    private static string Body(AggregateAttributeModel attribute)
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        var value = attribute.Name.ToCamelCase();
        var cached = value + "Cached";
        var contributors = value + "Contributors";
        var isValueType = attribute.Type.IsValueType;
        var notNull = attribute.Type is OptionalTypeModel || isValueType ? "" : "!";
        // Remove any optional type that might be on it and then ensure there is one optional type
        var fieldType = !isValueType ? attribute.Type.ToOptional() : attribute.Type;
        var syncLock = isValueType ? " ref syncLock," : "";
        builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}{notNull}");
        builder.AppendLine($"            : this.Aggregate(ref {contributors}, ref {cached}, ref {value},{syncLock}");
        builder.AppendLine($"                CollectContributors_{attribute.Name}, Collect_{attribute.Name});");
        builder.AppendLine($"    private {Type(fieldType)} {value};");
        builder.AppendLine($"    private bool {cached};");
        builder.Append($"    private IFixedSet<{BaseClassName(attribute.Aspect.Tree)}>? {contributors};");
        return builder.ToString();
    }

    private static string Body(CollectionAttributeModel attribute)
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        var value = attribute.Name.ToCamelCase();
        var cached = value + "Cached";
        var contributors = value + "Contributors";
        var isValueType = attribute.Type.IsValueType;
        var notNull = attribute.Type is OptionalTypeModel || isValueType ? "" : "!";
        // Remove any optional type that might be on it and then ensure there is one optional type
        var fieldType = !isValueType ? attribute.Type.ToOptional() : attribute.Type;
        var syncLock = isValueType ? " ref syncLock," : "";
        var rootType = attribute.RootSymbol is not null ? $"<{TypeName(attribute.RootSymbol)}>" : "";
        builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}{notNull}");
        builder.AppendLine($"            : this.Collection(ref {contributors}, ref {cached}, ref {value},{syncLock}");
        builder.AppendLine($"                CollectContributors_{attribute.Name}{rootType}, Collect_{attribute.Name});");
        builder.AppendLine($"    private {Type(fieldType)} {value};");
        builder.AppendLine($"    private bool {cached};");
        builder.Append($"    private IFixedSet<{BaseClassName(attribute.Aspect.Tree)}>? {contributors};");
        return builder.ToString();
    }
    #endregion

    #region Attribute Families
    public static string ContributeMethodName(AggregateAttributeFamilyModel family, TreeNodeModel node)
    {
        var contributesToThis = node.ActualAttributes.OfType<AggregateAttributeModel>()
                                    .Any(a => a.Family == family);
        var thisString = contributesToThis ? "This_" : "";
        return $"Contribute_{thisString}{family.Name}";
    }

    public static string RootParam(CollectionAttributeFamilyModel family)
        => family.HasRoot ? "<TRoot>" : "";
    #endregion

    #region Equations
    public static string Parameters(EquationModel equation)
    {
        if (equation is IntertypeMethodEquationModel method)
            return $"({method.Parameters})";
        return equation.IsMethod ? "()" : "";
    }

    public static string Override(LocalAttributeEquationModel equation)
        => equation.IsObjectMember() ? "override " : "";

    public static string ParametersAndBody(SoleEquationModel equation)
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
                var attribute = equation.Attribute;
                var isChild = attribute.IsChild;
                var attachStart = isChild ? $"n => {ChildAttach(equation)}" : "";
                var attachEnd = isChild ? "(n))" : "";
                if (attribute is CircularAttributeModel attr)
                {
                    // Circular attribute
                    builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}.UnsafeValue");
                    builder.AppendLine($"            : this.Circular(ref {cached}, ref {value},");
                    builder.Append("                ");
                    AppendQualifiedEquationMethod(equation, builder);
                    if (attr.InitialExpression is null)
                    {
                        builder.AppendLine(",");
                        builder.Append("                ");
                        AppendQualifiedInitialMethod(builder, attribute);
                    }
                    builder.AppendLine(");");
                    builder.Append($"    private Circular<{Type(equation.Type)}> {value}");
                    if (attr.InitialExpression is null)
                        builder.AppendLine(" = Circular.Unset;");
                    else
                        builder.AppendLine($" = new({attr.InitialExpression});");
                }
                else
                {
                    // Non-circular attribute
                    var isValueType = equation.Type.IsValueType;
                    // Remove any optional type that might be on it and then ensure there is one optional type
                    var fieldType = !isValueType ? equation.Type.ToOptional() : equation.Type;
                    var syncLock = isValueType ? " ref syncLock," : "";
                    var notNull = equation.Type is OptionalTypeModel || isValueType ? "" : "!";
                    builder.AppendLine($"        => GrammarAttribute.IsCached(in {cached}) ? {value}{notNull}");
                    builder.AppendLine($"            : this.Synthetic(ref {cached}, ref {value},{syncLock}");
                    builder.Append("                ");
                    builder.Append(attachStart);
                    AppendQualifiedEquationMethod(equation, builder);
                    builder.Append(attachEnd);
                    builder.AppendLine(");");
                    builder.AppendLine($"    private {Type(fieldType)} {value};");
                }

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

    public static string InitialMethod(CircularAttributeModel attribute)
    {
        var builder = new StringBuilder();
        AppendInitialMethod(builder, attribute);
        return builder.ToString();
    }

    private static void AppendQualifiedInitialMethod(StringBuilder builder, AspectAttributeModel attribute)
    {
        builder.Append(attribute.Aspect.Name);
        builder.Append('.');
        AppendInitialMethod(builder, attribute);
    }

    private static void AppendInitialMethod(StringBuilder builder, AspectAttributeModel attribute)
    {
        builder.Append(attribute.NodeSymbol);
        builder.Append('_');
        builder.Append(attribute.Name);
        builder.Append("_Initial");
    }

    private static void AppendQualifiedEquationMethod(EquationModel equation, StringBuilder builder)
    {
        builder.Append(equation.Aspect.Name);
        builder.Append('.');
        AppendEquationMethod(equation, builder);
    }

    public static string EquationMethod(LocalAttributeEquationModel equation)
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

    public static string EagerBody(LocalAttributeEquationModel equation)
    {
        Requires.That(equation.Strategy == EvaluationStrategy.Eager, nameof(equation), "Must be an eager equation.");
        if (equation.Expression is not null)
            throw new NotImplementedException("Eager equations with expressions are not yet"
                                              + " implemented. They should emit a protected method to the interface and call that"
                                              + " from the constructor.");

        var body = $"{equation.Aspect.Name}.{equation.NodeSymbol}_{equation.Name}(this)";
        if (equation.Attribute.IsChild)
            return $"{ChildAttach(equation)}{body})";

        return body;
    }

    public static string Selector(InheritedAttributeEquationModel equation)
    {
        var selector = equation.Selector;
        var child = equation.Node.ActualAttributes.Where(selector.Matches).TrySingle();
        var childName = child?.CurrentName;
        return selector switch
        {
            AllChildrenSelectorModel s
                => s.IsBroadcast ? "" // If broadcast, always matches, no condition needed
                    : SelectorIf("ReferenceEquals(child, descendant)"),
            ChildSelectorModel s => SelectorIf($"ReferenceEquals({ChildOrDescendant(s)}, Self.{childName})"),
            ChildAtIndexSelectorModel s => SelectorIf($"{s.Index} < Self.{childName}.Count && ReferenceEquals({ChildOrDescendant(s)}, Self.{childName}[{s.Index}])"),
            ChildAtVariableSelectorModel s => SelectorIf($"IndexOfNode(Self.{childName}, {ChildOrDescendant(s)}) is {{ }} {s.Variable}"),
            ChildListSelectorModel s => SelectorIf($"ContainsNode(Self.{childName}, {ChildOrDescendant(s)})"),
            _ => throw ExhaustiveMatch.Failed(selector)
        };
    }

    private static string SelectorIf(string condition)
        => "if (" + condition + $"){Environment.NewLine}            ";

    private static string ChildOrDescendant(SelectorModel selector)
        // Broadcast matches the child because it applies to all descendants under that child.
        // Whereas non-broadcast matches the descendant because the descendant must be a child.
        => selector.IsBroadcast ? "child" : "descendant";

    public static string Body(InheritedAttributeEquationModel equation)
    {
        if (equation.Expression is not null)
        {
            var typeMatchesReturnType = equation.Type == equation.AttributeFamily.Type;
            return typeMatchesReturnType ? equation.Expression
                : $"Is.OfType<{Type(equation.Type)}>({equation.Expression})";
        }

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
        } + (equation.Selector.IsBroadcast ? "_Broadcast" : "") + $"_{equation.Name}";

    public static string EquationMethodExtraParams(InheritedAttributeEquationModel equation)
    {
        if (equation.Selector is ChildAtVariableSelectorModel s)
            return $", int {s.Variable}";
        return "";
    }

    public static string Body(PreviousAttributeEquationModel equation)
    {
        if (equation.Expression is not null)
            return equation.Expression;

        return $"{QualifiedEquationMethod(equation)}(this)";
    }

    public static string QualifiedEquationMethod(PreviousAttributeEquationModel equation)
        => $"{equation.Aspect.Name}.{EquationMethod(equation)}";

    public static string EquationMethod(PreviousAttributeEquationModel equation)
        => $"{equation.NodeSymbol}_Next_{equation.Name}";

    public static string EquationMethod(AggregateAttributeEquationModel equation)
        => $"{equation.NodeSymbol}_Contribute_{equation.Name}";

    public static string QualifiedEquationMethod(AggregateAttributeEquationModel equation)
        => $"{equation.Aspect.Name}.{EquationMethod(equation)}";

    public static string EquationMethodExtraParams(AggregateAttributeEquationModel equation)
        => $", {Type(equation.FromType)} {equation.Name.ToCamelCase()}";

    public static string EquationMethod(CollectionAttributeEquationModel equation)
        => $"{equation.NodeSymbol}_Contribute_{equation.TargetNodeSymbol}_{equation.Name}";

    public static string EquationMethodExtraParams(CollectionAttributeEquationModel equation)
        => $", {TypeName(equation.TargetNodeSymbol)} target, {Type(equation.FromType)} {equation.Name.ToCamelCase()}";

    public static string AddContributors(CollectionAttributeEquationModel equation)
    {
        var tree = equation.Aspect.Tree;
        if (equation.TargetExpression is null)
            // TODO perhaps this should be restricted based on the target type?
            return $"contributors.AddToAll(({BaseClassName(tree)})this);";
        var range = equation.IsForEach ? "Range" : "";
        var cast = equation.IsForEach ? $".Cast<{BaseClassName(tree)}>()" : "";
        return $"contributors.AddTo{range}({equation.TargetExpression}{cast}, this);";
    }

    public static string Contribute(CollectionAttributeEquationModel equation)
        => $"if (target is {TypeName(equation.TargetNodeSymbol)} t){Environment.NewLine}"
           + $"            {equation.Aspect.Name}.{EquationMethod(equation)}(this, t, builder);";
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
