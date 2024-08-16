using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class LoopExpressionSyntax : ExpressionSyntax, ILoopExpressionSyntax
{
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }

    public LoopExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
        : base(span)
    {
        Block = block;
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"loop {Block}";
}
