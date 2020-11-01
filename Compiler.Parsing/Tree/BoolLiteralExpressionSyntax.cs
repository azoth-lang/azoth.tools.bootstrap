using System.Diagnostics;
using System.Globalization;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class BoolLiteralExpressionSyntax : LiteralExpressionSyntax, IBoolLiteralExpressionSyntax
    {
        public bool Value { [DebuggerStepThrough] get; }

        public BoolLiteralExpressionSyntax(TextSpan span, bool value)
            : base(span, ExpressionSemantics.Copy)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
