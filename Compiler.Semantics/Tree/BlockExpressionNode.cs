using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BlockExpressionNode : ExpressionNode, IBlockExpressionNode
{
    public override IBlockExpressionSyntax Syntax { get; }
    public IFixedList<IStatementNode> Statements { get; }
    private ValueAttribute<IMaybeAntetype> antetype;
    public override IMaybeAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.BlockExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.BlockExpression_Type, ref type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : GrammarAttribute.Circular(ref flowStateAfterCached, this,
                ExpressionTypesAspect.BlockExpression_FlowStateAfter, ref flowStateAfter);

    public BlockExpressionNode(IBlockExpressionSyntax syntax, IEnumerable<IStatementNode> statements)
    {
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
        => LexicalScopingAspect.BodyOrBlock_InheritedLexicalScope(this, Statements.IndexOf(child)!.Value);

    internal override FlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (Statements.IndexOf(child) is int index and > 0)
            return Statements[index - 1].FlowStateAfter;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    public FlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
}
