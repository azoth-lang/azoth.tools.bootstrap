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

    public static IMaybeAntetype BindingPattern_BindingAntetype(IBindingPatternNode node)
        => throw new System.NotImplementedException();

    public static IMaybeAntetype VariableDeclarationStatement_BindingAntetype(IVariableDeclarationStatementNode node)
        => node.Type?.Antetype ?? node.FinalInitializer?.Antetype.ToNonConstValueType() ?? IAntetype.Unknown;

    public static IMaybeAntetype ForeachExpression_BindingAntetype(IForeachExpressionNode node)
        => throw new System.NotImplementedException();
}
