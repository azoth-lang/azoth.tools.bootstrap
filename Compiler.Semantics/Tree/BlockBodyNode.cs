using System.Collections.Generic;
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
    public IFlowState FlowStateAfter
        => throw new System.NotImplementedException();

    public BlockBodyNode(IBlockBodySyntax syntax, IEnumerable<IBodyStatementNode> statements)
    {
        Syntax = syntax;
        Statements = ChildList.Attach(this, statements);
    }

    public LexicalScope GetContainingLexicalScope()
        => InheritedContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

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
}
