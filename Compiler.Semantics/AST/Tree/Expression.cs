using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal abstract class Expression : AbstractSyntax, IExpression
    {
        public DataType DataType { get; }
        public ExpressionSemantics Semantics { get; }

        protected Expression(TextSpan span, DataType dataType, ExpressionSemantics semantics)
            : base(span)
        {
            DataType = dataType;
            Semantics = semantics;
        }

        protected abstract OperatorPrecedence ExpressionPrecedence { get; }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
        }
    }
}
