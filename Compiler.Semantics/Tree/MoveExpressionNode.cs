using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MoveExpressionNode : ExpressionNode, IMoveExpressionNode
{
    public override IMoveExpressionSyntax Syntax { get; }
    public IVariableNameExpressionNode Referent { get; }

    public MoveExpressionNode(IMoveExpressionSyntax syntax, IVariableNameExpressionNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}
