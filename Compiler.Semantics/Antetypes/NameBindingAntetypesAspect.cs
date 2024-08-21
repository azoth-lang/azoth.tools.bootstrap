using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class NameBindingAntetypesAspect
{
    public static IMaybeAntetype FieldParameter_BindingAntetype(IFieldParameterNode node)
        => node.ReferencedField?.BindingAntetype ?? IAntetype.Unknown;

    public static IMaybeAntetype SelfParameter_BindingAntetype(ISelfParameterNode node)
    {
        var containingDeclaredAntetype = node.ContainingTypeDefinition.DeclaredType.ToAntetype();
        return containingDeclaredAntetype.With(containingDeclaredAntetype.GenericParameterAntetypes);
    }

    public static IMaybeAntetype PatternMatchExpression_Pattern_ContextBindingAntetype_(IPatternMatchExpressionNode node)
        => node.IntermediateReferent?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static IMaybeAntetype BindingContextPattern_Pattern_ContextBindingAntetype(IBindingContextPatternNode node)
        => node.Type?.NamedAntetype ?? node.ContextBindingAntetype();

    public static IMaybeAntetype OptionalPattern_Pattern_ContextBindingAntetype(
        IOptionalPatternNode node)
    {
        var inheritedBindingAntetype = node.ContextBindingAntetype();
        if (inheritedBindingAntetype is OptionalAntetype optionalAntetype)
            return optionalAntetype.Referent;
        return inheritedBindingAntetype;
    }

    public static IMaybeAntetype BindingPattern_BindingAntetype(IBindingPatternNode node)
        => node.ContextBindingAntetype();

    public static IMaybeAntetype VariableDeclarationStatement_BindingAntetype(IVariableDeclarationStatementNode node)
        => node.Type?.NamedAntetype ?? node.IntermediateInitializer?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static void VariableDeclarationStatement_ContributeDiagnostics(
        IVariableDeclarationStatementNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Type is null && node.IntermediateInitializer is null)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.NameSpan,
                "Inference of local variable types not implemented"));
    }

    public static IMaybeAntetype ForeachExpression_BindingAntetype(IForeachExpressionNode node)
        => node.DeclaredType?.NamedAntetype ?? node.IteratedAntetype;

    public static IMaybeAntetype NewObjectExpression_ConstructingAntetype(INewObjectExpressionNode node)
        => node.ConstructingType.NamedAntetype;
}
