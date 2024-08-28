using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
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
    private DataType? expectedType;
    private bool expectedTypeCached;
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType, InheritedExpectedType);
    private readonly IFixedList<IStatementNode> statements;
    IFixedList<IStatementNode> IBodyOrBlockNode.Statements => statements;
    public IFlowState FlowStateAfter => throw new NotImplementedException();

    public ExpressionBodyNode(IExpressionBodySyntax syntax, IResultStatementNode resultStatement)
    {
        Syntax = syntax;
        ResultStatement = Child.Attach(this, resultStatement);
        statements = FixedList.Create(ResultStatement);
    }

    public LexicalScope ContainingLexicalScope()
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    internal override LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        var statementIndex = child == ResultStatement ? 0 : throw new InvalidOperationException("Caller is not a child");
        return LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, statementIndex);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == ResultStatement)
            return ExpectedAntetype;
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == ResultStatement)
            return ExpectedType;
        return base.InheritedExpectedType(child, descendant, ctx);
    }

    internal override bool Inherited_ImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == ResultStatement.CurrentExpression) return true;
        return false;
    }

    internal override bool Inherited_ShouldPrepareToReturn(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == ResultStatement.CurrentExpression) return true;
        return false;
    }
}
