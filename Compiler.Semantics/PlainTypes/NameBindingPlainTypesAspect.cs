using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class NameBindingPlainTypesAspect
{
    // TODO this is strange because a FieldParameter isn't a binding
    public static partial IMaybeNonVoidPlainType FieldParameter_BindingPlainType(IFieldParameterNode node)
        => node.ReferencedField?.BindingPlainType ?? PlainType.Unknown;

    public static partial ConstructedPlainType SelfParameter_BindingPlainType(ISelfParameterNode node)
        => node.ContainingSelfTypeConstructor.ConstructWithParameterPlainTypes();

    public static partial IMaybeNonVoidPlainType PatternMatchExpression_Pattern_ContextBindingPlainType(IPatternMatchExpressionNode node)
        => node.Referent?.PlainType.ToNonLiteral().ToNonVoid() ?? PlainType.Unknown;

    public static partial IMaybeNonVoidPlainType BindingContextPattern_Pattern_ContextBindingPlainType(IBindingContextPatternNode node)
        => node.Type?.NamedPlainType.ToNonVoid() ?? node.ContextBindingPlainType();

    public static partial IMaybeNonVoidPlainType OptionalPattern_Pattern_ContextBindingPlainType(
        IOptionalPatternNode node)
    {
        var inheritedBindingPlainType = node.ContextBindingPlainType();
        if (inheritedBindingPlainType is OptionalPlainType optionalPlainType)
            return optionalPlainType.Referent;
        return inheritedBindingPlainType;
    }

    public static partial IMaybeNonVoidPlainType BindingPattern_BindingPlainType(IBindingPatternNode node)
        => node.ContextBindingPlainType();

    public static partial IMaybeNonVoidPlainType VariableDeclarationStatement_BindingPlainType(IVariableDeclarationStatementNode node)
        => node.Type?.NamedPlainType.ToNonVoid() ?? node.Initializer?.PlainType.ToNonLiteral().ToNonVoid() ?? PlainType.Unknown;

    public static partial void VariableDeclarationStatement_Contribute_Diagnostics(
        IVariableDeclarationStatementNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Type is null && node.Initializer is null)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.NameSpan,
                "Inference of local variable types not implemented"));
    }

    public static partial IMaybeNonVoidPlainType ForeachExpression_BindingPlainType(IForeachExpressionNode node)
        => node.DeclaredType?.NamedPlainType.ToNonVoid() ?? node.IteratedPlainType;

    public static partial IMaybeNonVoidPlainType NewObjectExpression_ConstructingPlainType(INewObjectExpressionNode node)
        => node.ConstructingType.NamedPlainType.ToNonVoid();

    public static partial IMaybeNonVoidPlainType NamedParameter_BindingPlainType(INamedParameterNode node)
        => node.TypeNode.NamedPlainType.ToNonVoid();
}
