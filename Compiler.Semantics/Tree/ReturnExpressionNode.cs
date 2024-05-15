using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ReturnExpressionNode : ExpressionNode, IReturnExpressionNode
{
    public override IReturnExpressionSyntax Syntax { get; }
    public NeverType Type => DataType.Never;
    public IUntypedExpressionNode? Value { get; }

    public ReturnExpressionNode(IReturnExpressionSyntax syntax, IUntypedExpressionNode? value)
    {
        Syntax = syntax;
        Value = Child.Attach(this, value);
    }
}
