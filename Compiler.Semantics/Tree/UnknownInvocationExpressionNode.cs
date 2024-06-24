using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnknownInvocationExpressionNode : ExpressionNode, IUnknownInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }
    public override IMaybeExpressionAntetype Antetype => IAntetype.Unknown;

    public UnknownInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Legacy(this, expression);
        Arguments = ChildList.Create(this, arguments);
    }
}
