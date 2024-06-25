using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class NameBindingAntetypesAspect
{
    public static IMaybeAntetype FieldParameter_BindingAntetype(IFieldParameterNode node)
        => node.ReferencedField?.BindingAntetype ?? IAntetype.Unknown;

    public static IMaybeAntetype SelfParameter_BindingAntetype(ISelfParameterNode node)
    {
        var containingDeclaredAntetype = node.ContainingTypeDefinition.DeclaredType.ToAntetype();
        return containingDeclaredAntetype.With(containingDeclaredAntetype.GenericParameterAntetypes);
    }

    public static IMaybeAntetype PatternMatchExpression_InheritedBindingAntetype_Pattern(IPatternMatchExpressionNode node)
        => node.IntermediateReferent?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static IMaybeAntetype BindingContextPattern_InheritedBindingAntetype_Pattern(IBindingContextPatternNode node)
        => node.Type?.NamedAntetype ?? node.InheritedBindingAntetype();

    public static IMaybeAntetype OptionalPattern_InheritedBindingAntetype_Pattern(
        IOptionalPatternNode node)
    {
        var inheritedBindingAntetype = node.InheritedBindingAntetype();
        if (inheritedBindingAntetype is OptionalAntetype optionalAntetype)
            return optionalAntetype.Referent;
        return inheritedBindingAntetype;
    }

    public static IMaybeAntetype BindingPattern_BindingAntetype(IBindingPatternNode node)
        => node.InheritedBindingAntetype();

    public static IMaybeAntetype VariableDeclarationStatement_BindingAntetype(IVariableDeclarationStatementNode node)
        => node.Type?.NamedAntetype ?? node.IntermediateInitializer?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static IMaybeAntetype ForeachExpression_BindingAntetype(IForeachExpressionNode node)
        => node.DeclaredType?.NamedAntetype ?? node.IteratedAntetype;

    public static IMaybeAntetype NewObjectExpression_ConstructingAntetype(INewObjectExpressionNode node)
        => node.ConstructingType.NamedAntetype;
}
