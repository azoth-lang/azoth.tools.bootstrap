using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BlockExpressionNode : ExpressionNode, IBlockExpressionNode
{
    public override IBlockExpressionSyntax Syntax { get; }
    ICodeSyntax IElseClauseNode.Syntax => Syntax;
    public IFixedList<IStatementNode> Statements { get; }
    private IMaybeAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.BlockExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.BlockExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.BlockExpression_FlowStateAfter);

    public BlockExpressionNode(IBlockExpressionSyntax syntax, IEnumerable<IStatementNode> statements)
    {
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    LexicalScope IBlockExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => LexicalScopingAspect.BodyOrBlock_Statements_Broadcast_ContainingLexicalScope(this, Statements.IndexOf((IChildNode)child)!.Value);

    internal override IFlowState Inherited_FlowStateBefore(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (Statements.IndexOf((IChildNode)child) is int index and > 0)
            return Statements[index - 1].FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics, bool contributeAttribute = true)
    {
        InvalidStructureAspect.BlockExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics, contributeAttribute);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.BlockExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child is IStatementNode statement && Statements.IndexOf(statement) is int index
                                              && index < Statements.Count - 1)
            return ControlFlowSet.CreateNormal(Statements[index + 1]);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }
}
