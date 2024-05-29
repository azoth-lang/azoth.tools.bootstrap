using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InvocationExpressionNode : AmbiguousExpressionNode, IInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;
    public IFixedList<IAmbiguousExpressionNode> Arguments { get; }

    public InvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        Arguments = ChildList.Create(this, arguments);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
    {
        if (child == Expression)
            return GetContainingLexicalScope();
        if (Arguments.IndexOf(child) is int argumentIndex)
        {
            if (argumentIndex == 0)
                return Expression.GetFlowLexicalScope().True;

            return Arguments[argumentIndex - 1].GetFlowLexicalScope().True;
        }
        return base.InheritedContainingLexicalScope(child, descendant);
    }
}
