using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IfExpressionNode : ExpressionNode, IIfExpressionNode
{
    public override IIfExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> condition;
    public IUntypedExpressionNode Condition => condition.Value;
    public IBlockOrResultNode ThenBlock { get; }
    public IElseClauseNode? ElseClause { get; }

    public IfExpressionNode(
        IIfExpressionSyntax syntax,
        IUntypedExpressionNode condition,
        IBlockOrResultNode thenBlock,
        IElseClauseNode? elseClause)
    {
        Syntax = syntax;
        this.condition = Child.Create(this, condition);
        ThenBlock = Child.Attach(this, thenBlock);
        ElseClause = Child.Attach(this, elseClause);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == ThenBlock)
            return Condition.GetFlowLexicalScope().True;
        if (child == ElseClause)
            return Condition.GetFlowLexicalScope().False;
        return base.InheritedContainingLexicalScope(child, descendant);
    }
}
