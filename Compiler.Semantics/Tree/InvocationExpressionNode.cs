using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InvocationExpressionNode : ExpressionNode, IInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    private Child<IUntypedExpressionNode> expression;
    public IUntypedExpressionNode Expression => expression.Value;
    public IFixedList<IUntypedExpressionNode> Arguments { get; }

    public InvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IUntypedExpressionNode expression,
        IEnumerable<IUntypedExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        Arguments = ChildList.Create(this, arguments);
    }
}
