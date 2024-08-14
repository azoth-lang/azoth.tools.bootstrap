using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BlockExpressionNode : ExpressionNode, IBlockExpressionNode
{
    public override IBlockExpressionSyntax Syntax { get; }
    IConcreteSyntax IElseClauseNode.Syntax => Syntax;
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

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => LexicalScopingAspect.BodyOrBlock_InheritedLexicalScope(this, Statements.IndexOf(child)!.Value);

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (Statements.IndexOf(child) is int index and > 0)
            return Statements[index - 1].FlowStateAfter;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    public IFlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    protected override void CollectDiagnostics(DiagnosticsBuilder diagnostics)
    {
        InvalidStructureAspect.BlockExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.BlockExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child is IStatementNode statement && Statements.IndexOf(statement) is int index
                                              && index < Statements.Count - 1)
            return ControlFlowSet.CreateNormal(Statements[index + 1]);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }
}
