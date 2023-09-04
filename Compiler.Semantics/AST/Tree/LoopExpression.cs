using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class LoopExpression : Expression, ILoopExpression
{
    public IBlockExpression Block { get; }

    public LoopExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IBlockExpression block)
        : base(span, dataType, semantics)
    {
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"loop {Block}";
}
