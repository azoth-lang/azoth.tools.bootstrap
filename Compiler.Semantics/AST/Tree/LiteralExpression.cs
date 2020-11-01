using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal abstract class LiteralExpression : Expression, ILiteralExpression
    {
        protected LiteralExpression(TextSpan span, DataType dataType, ExpressionSemantics semantics)
            : base(span, dataType, semantics) { }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    }
}
