using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MoveExpressionNode : ExpressionNode, IMoveExpressionNode
{
    public override IMoveExpressionSyntax Syntax { get; }
    private Child<IVariableNameExpressionNode> referent;
    public IVariableNameExpressionNode Referent => referent.Value;

    public MoveExpressionNode(IMoveExpressionSyntax syntax, IVariableNameExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }
}
