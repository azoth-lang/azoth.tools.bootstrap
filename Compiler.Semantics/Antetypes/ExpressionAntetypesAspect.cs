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
}
