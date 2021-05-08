using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class ImplicitLiftedConversion : ImplicitConversionExpression, IImplicitLiftedConversionExpression
    {
        public OptionalType ConvertToType { get; }

        public ImplicitLiftedConversion(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression expression,
            OptionalType convertToType)
            : base(span, dataType, semantics, expression)
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as ⟦Lifted?⟧⟧";
        }
    }
}
