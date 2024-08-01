using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExpressionBodyNode : CodeNode, IExpressionBodyNode
{
    public override IExpressionBodySyntax Syntax { get; }
    public IResultStatementNode ResultStatement { get; }
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype, InheritedExpectedAntetype);
    private readonly IFixedList<IStatementNode> statements;
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => statements;
    public IFlowState FlowStateAfter => throw new NotImplementedException();

    public ExpressionBodyNode(IExpressionBodySyntax syntax, IResultStatementNode resultStatement)
    {
        Syntax = syntax;
        ResultStatement = Child.Attach(this, resultStatement);
        statements = FixedList.Create(ResultStatement);
    }

    public LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        var statementIndex = child == ResultStatement ? 0 : throw new InvalidOperationException("Caller is not a child");
        return LexicalScopingAspect.BodyOrBlock_InheritedLexicalScope(this, statementIndex);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == ResultStatement)
            return ExpectedAntetype;
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }
}
