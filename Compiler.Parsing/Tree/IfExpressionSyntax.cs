using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class IfExpressionSyntax : DataTypedExpressionSyntax, IIfExpressionSyntax
{
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockOrResultSyntax ThenBlock { [DebuggerStepThrough] get; }
    public IElseClauseSyntax? ElseClause { [DebuggerStepThrough] get; }

    public IfExpressionSyntax(
        TextSpan span,
        IExpressionSyntax condition,
        IBlockOrResultSyntax thenBlock,
        IElseClauseSyntax? elseClause)
        : base(span)
    {
        Condition = condition;
        ThenBlock = thenBlock;
        ElseClause = elseClause;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
    {
        if (ElseClause is not null)
            return $"if {Condition} {ThenBlock} else {ElseClause}";
        return $"if {Condition} {ThenBlock}";
    }
}
