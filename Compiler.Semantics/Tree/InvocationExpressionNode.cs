using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
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

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Expression)
            return GetContainingLexicalScope();
        var argumentIndex = Arguments.IndexOf(child) ?? throw new ArgumentException("Is not a child of this node.", nameof(child));
        if (argumentIndex == 0)
            return Expression.GetFlowLexicalScope().True;

        return Arguments[argumentIndex - 1].GetFlowLexicalScope().True;
    }
}
