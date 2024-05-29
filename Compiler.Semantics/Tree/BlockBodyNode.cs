using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BlockBodyNode : CodeNode, IBlockBodyNode
{
    public override IBlockBodySyntax Syntax { get; }
    public IFixedList<IBodyStatementNode> Statements { get; }
    public ValueId? ValueId => null;
    public FlowState FlowStateAfter
        => Statements.LastOrDefault()?.Predecessor().FlowStateAfter ?? FlowStateBefore();

    public BlockBodyNode(IBlockBodySyntax syntax, IEnumerable<IBodyStatementNode> statements)
    {
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    public LexicalScope GetContainingLexicalScope() => InheritedContainingLexicalScope();

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
        => LexicalScopingAspect.BodyOrBlock_InheritedLexicalScope(this, Statements.IndexOf(child)!.Value);

    public IFlowNode Predecessor() => InheritedPredecessor();
    public FlowState FlowStateBefore() => Predecessor().FlowStateAfter;

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (Statements.IndexOf(child) is int index)
            if (index > 0)
                return Statements[index - 1].Predecessor();
            else
                return base.InheritedPredecessor(child, descendant);

        return base.InheritedPredecessor(child, descendant);
    }
}
