using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal abstract class ImplicitConversionExpression : Expression, IImplicitConversionExpression
    {
        public IExpression Expression { get; }

        protected ImplicitConversionExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression expression)
            : base(span, dataType, semantics)
        {
            Expression = expression;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;
    }
}
