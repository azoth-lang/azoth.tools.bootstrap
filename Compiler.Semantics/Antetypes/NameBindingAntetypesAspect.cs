using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class NameBindingAntetypesAspect
{
    public static partial IMaybeAntetype FieldParameter_BindingAntetype(IFieldParameterNode node)
        => node.ReferencedField?.BindingAntetype ?? IAntetype.Unknown;

    public static partial IMaybeAntetype SelfParameter_BindingAntetype(ISelfParameterNode node)
    {
        var containingDeclaredAntetype = node.ContainingTypeDefinition.DeclaredType.ToAntetype();
        return containingDeclaredAntetype.With(containingDeclaredAntetype.GenericParameterAntetypes);
    }

    public static IMaybeAntetype PatternMatchExpression_Pattern_ContextBindingAntetype_(IPatternMatchExpressionNode node)
        => node.Referent?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

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

    public static partial IMaybeAntetype BindingPattern_BindingAntetype(IBindingPatternNode node)
        => node.ContextBindingAntetype();

    public static partial IMaybeAntetype VariableDeclarationStatement_BindingAntetype(IVariableDeclarationStatementNode node)
        => node.Type?.NamedAntetype ?? node.Initializer?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static void VariableDeclarationStatement_ContributeDiagnostics(
        IVariableDeclarationStatementNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Type is null && node.Initializer is null)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.NameSpan,
                "Inference of local variable types not implemented"));
    }

    public static partial IMaybeAntetype ForeachExpression_BindingAntetype(IForeachExpressionNode node)
        => node.DeclaredType?.NamedAntetype ?? node.IteratedAntetype;

    public static IMaybeAntetype NewObjectExpression_ConstructingAntetype(INewObjectExpressionNode node)
        => node.ConstructingType.NamedAntetype;

    public static partial IMaybeAntetype NamedParameter_BindingAntetype(INamedParameterNode node)
        => node.TypeNode.NamedAntetype;
}
