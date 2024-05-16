using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FreezeExpressionNode : ExpressionNode, IFreezeExpressionNode
{
    public override IFreezeExpressionSyntax Syntax { get; }
    private Child<IVariableNameExpressionNode> referent;
    public IVariableNameExpressionNode Referent => referent.Value;

    public FreezeExpressionNode(IFreezeExpressionSyntax syntax, IVariableNameExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }
}
