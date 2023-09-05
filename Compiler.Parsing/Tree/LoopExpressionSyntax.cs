using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class LoopExpressionSyntax : ExpressionSyntax, ILoopExpressionSyntax
{
    public IBlockExpressionSyntax Block { get; }

    public LoopExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
        : base(span)
    {
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"loop {Block}";
}
