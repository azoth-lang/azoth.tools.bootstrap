using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class LoopExpressionSyntax : DataTypedExpressionSyntax, ILoopExpressionSyntax
{
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }

    public LoopExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
        : base(span)
    {
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"loop {Block}";
}
