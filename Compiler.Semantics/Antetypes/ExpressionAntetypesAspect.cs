using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class ExpressionAntetypesAspect
{
    public static IMaybeExpressionAntetype UnsafeExpression_Antetype(IUnsafeExpressionNode node)
        => node.FinalExpression.Antetype;

    public static IMaybeExpressionAntetype FunctionInvocationExpression_Antetype(IFunctionInvocationExpressionNode node)
        // TODO should probably use Antetype on the declaration
        => node.ReferencedDeclaration?.Type.Return.Type.ToAntetype() ?? IAntetype.Unknown;

    public static IMaybeExpressionAntetype VariableNameExpression_Antetype(IVariableNameExpressionNode node)
        => node.ReferencedDeclaration.BindingAntetype;

    public static IMaybeExpressionAntetype SelfExpression_Antetype(ISelfExpressionNode node)
        => node.ReferencedParameter?.Antetype ?? IAntetype.Unknown;

    public static IMaybeExpressionAntetype FieldAccessExpression_Antetype(IFieldAccessExpressionNode node)
    {
        // TODO should probably use Antetype on the declaration
        var fieldAntetype = node.ReferencedDeclaration.Type.ToAntetype();
        // TODO replace type parameters with actual types
        return fieldAntetype;
    }

    public static IMaybeExpressionAntetype NewObjectExpression_Antetype(INewObjectExpressionNode node)
        // TODO should probably use Antetype on the declaration
        // TODO replace type parameters with actual types
        => node.ReferencedConstructor?.Symbol.ReturnType.ToAntetype() ?? IAntetype.Unknown;
}
