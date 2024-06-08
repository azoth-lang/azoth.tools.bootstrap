using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class NameBindingAntetypesAspect
{
    public static IMaybeAntetype FieldParameter_Antetype(IFieldParameterNode node)
        => node.ReferencedField?.Antetype ?? IAntetype.Unknown;

    public static IMaybeAntetype SelfParameter_Antetype(ISelfParameterNode node)
    {
        var containingDeclaredAntetype = node.ContainingTypeDefinition.DeclaredType.ToAntetype();
        return containingDeclaredAntetype.With(containingDeclaredAntetype.GenericParameterAntetypes);
    }

    public static IMaybeAntetype PatternMatchExpression_InheritedBindingAntetype_Pattern(IPatternMatchExpressionNode node)
        => node.FinalReferent.Antetype.ToNonConstValueType();

    public static IMaybeAntetype BindingContextPattern_InheritedBindingAntetype_Pattern(IBindingContextPatternNode node)
        => node.Type?.Antetype ?? node.InheritedBindingAntetype();

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
        => node.Type?.Antetype ?? node.FinalInitializer?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static IMaybeAntetype ForeachExpression_BindingAntetype(IForeachExpressionNode node)
        => node.DeclaredType?.Antetype ?? node.IteratedAntetype;

    public static IMaybeAntetype NewObjectExpression_ConstructingAntetype(INewObjectExpressionNode node)
        => node.ConstructingType.Antetype;
}
