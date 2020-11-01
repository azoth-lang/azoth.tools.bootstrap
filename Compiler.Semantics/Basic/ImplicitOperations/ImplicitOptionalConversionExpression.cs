using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.ImplicitOperations
{
    /// <summary>
    /// An implicit conversion from `T` to `T?`
    /// </summary>
    internal class ImplicitOptionalConversionExpression : ImplicitConversionExpression, IImplicitOptionalConversionExpressionSyntax
    {
        public OptionalType ConvertToType { get; }

        public ImplicitOptionalConversionExpression(
            IExpressionSyntax expression,
            OptionalType convertToType)
            : base(expression.Span, convertToType, expression, expression.Semantics.Assigned())
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as {ConvertToType}⟧";
        }
    }
}
