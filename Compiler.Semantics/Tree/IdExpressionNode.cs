using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdExpressionNode : ExpressionNode, IIdExpressionNode
{
    public override IIdExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> referent;
    public IUntypedExpressionNode Referent => referent.Value;

    public IdExpressionNode(IIdExpressionSyntax syntax, IUntypedExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }
}
