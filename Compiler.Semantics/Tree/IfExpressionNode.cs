using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IfExpressionNode : ExpressionNode, IIfExpressionNode
{
    public override IIfExpressionSyntax Syntax { get; }
    public IUntypedExpressionNode Condition { get; }
    public IBlockOrResultNode ThenBlock { get; }
    public IElseClauseNode? ElseClause { get; }

    public IfExpressionNode(
        IIfExpressionSyntax syntax,
        IUntypedExpressionNode condition,
        IBlockOrResultNode thenBlock,
        IElseClauseNode? elseClause)
    {
        Syntax = syntax;
        Condition = Child.Attach(this, condition);
        ThenBlock = Child.Attach(this, thenBlock);
        ElseClause = Child.Attach(this, elseClause);
    }
}
