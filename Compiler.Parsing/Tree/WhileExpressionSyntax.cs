using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class WhileExpressionSyntax : ExpressionSyntax, IWhileExpressionSyntax
{
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }

    public WhileExpressionSyntax(
        TextSpan span,
        IExpressionSyntax condition,
        IBlockExpressionSyntax block)
        : base(span)
    {
        Condition = condition;
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"while {Condition} {Block}";
}
