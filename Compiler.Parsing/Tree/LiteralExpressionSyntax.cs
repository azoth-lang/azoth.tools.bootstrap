using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal abstract class LiteralExpressionSyntax : ExpressionSyntax, ILiteralExpressionSyntax
    {
        protected LiteralExpressionSyntax(TextSpan span, ExpressionSemantics? semantics = null)
            : base(span, semantics)
        {
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    }
}
