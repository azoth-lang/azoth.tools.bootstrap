using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.ImplicitOperations
{
    // TODO No error is reported if IImplicitImmutabilityConversionExpression is missing
    internal class ImplicitImmutabilityConversionExpression : ImplicitConversionExpression, IImplicitImmutabilityConversionExpressionSyntax
    {
        public ObjectType ConvertToType { get; }

        public ImplicitImmutabilityConversionExpression(
            IExpressionSyntax expression,
            ObjectType convertToType)
            : base(expression.Span, convertToType, expression, expression.Semantics.Assigned())
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as ⟦immutable⟧⟧";
        }
    }
}
