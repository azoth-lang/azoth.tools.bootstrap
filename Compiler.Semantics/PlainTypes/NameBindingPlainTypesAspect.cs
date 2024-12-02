using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class NameBindingPlainTypesAspect
{
    // TODO this is strange because a FieldParameter isn't a binding
    public static partial IMaybePlainType FieldParameter_BindingPlainType(IFieldParameterNode node)
        => node.ReferencedField?.BindingPlainType ?? IPlainType.Unknown;

    public static partial IMaybePlainType SelfParameter_BindingPlainType(ISelfParameterNode node)
    {
        var containingDeclaredAntetype = node.ContainingTypeDefinition.DeclaredPlainType;
        return containingDeclaredAntetype.Construct(containingDeclaredAntetype.GenericParameterPlainTypes);
    }

    public static partial IMaybePlainType PatternMatchExpression_Pattern_ContextBindingPlainType(IPatternMatchExpressionNode node)
        => node.Referent?.PlainType.ToNonLiteralType() ?? IPlainType.Unknown;

    public static partial IMaybePlainType BindingContextPattern_Pattern_ContextBindingPlainType(IBindingContextPatternNode node)
        => node.Type?.NamedPlainType ?? node.ContextBindingPlainType();

    public static partial IMaybePlainType OptionalPattern_Pattern_ContextBindingPlainType(
        IOptionalPatternNode node)
    {
        var inheritedBindingAntetype = node.ContextBindingPlainType();
        if (inheritedBindingAntetype is OptionalPlainType optionalAntetype)
            return optionalAntetype.Referent;
        return inheritedBindingAntetype;
    }

    public static partial IMaybePlainType BindingPattern_BindingPlainType(IBindingPatternNode node)
        => node.ContextBindingPlainType();

    public static partial IMaybePlainType VariableDeclarationStatement_BindingPlainType(IVariableDeclarationStatementNode node)
        => node.Type?.NamedPlainType ?? node.Initializer?.PlainType.ToNonLiteralType() ?? IPlainType.Unknown;

    public static partial void VariableDeclarationStatement_Contribute_Diagnostics(
        IVariableDeclarationStatementNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Type is null && node.Initializer is null)
            diagnostics.Add(TypeError.NotImplemented(node.File, node.Syntax.NameSpan,
                "Inference of local variable types not implemented"));
    }

    public static partial IMaybePlainType ForeachExpression_BindingPlainType(IForeachExpressionNode node)
        => node.DeclaredType?.NamedPlainType ?? node.IteratedPlainType;

    public static partial IMaybePlainType NewObjectExpression_ConstructingPlainType(INewObjectExpressionNode node)
        => node.ConstructingType.NamedPlainType;

    public static partial IMaybePlainType NamedParameter_BindingPlainType(INamedParameterNode node)
        => node.TypeNode.NamedPlainType;
}
