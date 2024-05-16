using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BreakExpressionNode : ExpressionNode, IBreakExpressionNode
{
    public override IBreakExpressionSyntax Syntax { get; }
    public IUntypedExpressionNode? Value { get; }
    public NeverType Type => DataType.Never;

    public BreakExpressionNode(IBreakExpressionSyntax syntax, IUntypedExpressionNode? value)
    {
        Syntax = syntax;
        Value = Child.Attach(this, value);
    }
}