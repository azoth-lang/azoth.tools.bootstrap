using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class WhileExpression : Expression, IWhileExpression
    {
        public IExpression Condition { get; }
        public IBlockExpression Block { get; }

        public WhileExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression condition,
            IBlockExpression block)
            : base(span, dataType, semantics)
        {
            Condition = condition;
            Block = block;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return $"while {Condition} {Block}";
        }
    }
}
