using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FreezeExpressionNode : ExpressionNode, IFreezeExpressionNode
{
    public override IFreezeExpressionSyntax Syntax { get; }
    private Child<ISimpleNameNode> referent;
    public ISimpleNameNode Referent => referent.Value;

    public FreezeExpressionNode(IFreezeExpressionSyntax syntax, ISimpleNameNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == Referent)
            return Predecessor();
        return base.InheritedPredecessor(child, descendant);
    }
}
