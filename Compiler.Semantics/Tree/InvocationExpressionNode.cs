using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InvocationExpressionNode : AmbiguousExpressionNode, IInvocationExpressionNode
{
    protected override bool MayHaveRewrite => true;
    public override IInvocationExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;
    private readonly ChildList<IAmbiguousExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IEnumerable<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;

    public InvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        this.arguments = ChildList.Create(this, arguments);
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

    protected override IChildNode? Rewrite()
        => ExpressionTypesAspect.InvocationExpression_Rewrite_FunctionGroupNameExpression(this);
}
