using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdExpressionNode : ExpressionNode, IIdExpressionNode
{
    public override IIdExpressionSyntax Syntax { get; }
    public IUntypedExpressionNode Referent { get; }

    public IdExpressionNode(IIdExpressionSyntax syntax, IUntypedExpressionNode referent)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
    }
}
