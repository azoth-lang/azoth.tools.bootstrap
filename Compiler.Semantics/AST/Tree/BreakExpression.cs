using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class BreakExpression : Expression, IBreakExpression
    {
        public IExpression? Value { get; }

        public BreakExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression? value)
            : base(span, dataType, semantics)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence =>
            Value != null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

        public override string ToString()
        {
            if (Value != null) return $"break {Value}";
            return "break";
        }
    }
}
