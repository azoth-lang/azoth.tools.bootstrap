using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class NameBindingAntetypesAspect
{
    public static IMaybeAntetype FieldParameter_BindingAntetype(IFieldParameterNode node)
        => node.ReferencedField?.Antetype ?? IAntetype.Unknown;

    public static IMaybeAntetype SelfParameter_Antetype(ISelfParameterNode node)
        => throw new System.NotImplementedException();

    public static IMaybeAntetype BindingPattern_BindingAntetype(IBindingPatternNode node)
        => throw new System.NotImplementedException();

    public static IMaybeAntetype VariableDeclarationStatement_BindingAntetype(IVariableDeclarationStatementNode node)
        => throw new System.NotImplementedException();

    public static IMaybeAntetype ForeachExpression_BindingAntetype(IForeachExpressionNode node)
        => throw new System.NotImplementedException();
}
