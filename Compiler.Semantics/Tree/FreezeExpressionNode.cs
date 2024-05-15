using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FreezeExpressionNode : ExpressionNode, IFreezeExpressionNode
{
    public override IFreezeExpressionSyntax Syntax { get; }
    public IVariableNameExpressionNode Referent { get; }

    public FreezeExpressionNode(IFreezeExpressionSyntax syntax, IVariableNameExpressionNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}
