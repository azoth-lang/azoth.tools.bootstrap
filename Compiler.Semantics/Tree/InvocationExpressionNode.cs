using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class InvocationExpressionNode : AmbiguousExpressionNode, IInvocationExpressionNode
{
    protected override bool MayHaveRewrite => true;
    public override IInvocationExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode Expression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;

    private readonly RewritableChildList<InvocationExpressionNode, IAmbiguousExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;

    public InvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        this.arguments = ChildList.Create(this, nameof(Arguments), arguments);
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
        => OverloadResolutionAspect.InvocationExpression_Rewrite_FunctionGroupNameExpression(this)
        ?? OverloadResolutionAspect.InvocationExpression_Rewrite_MethodGroupNameExpression(this)
        ?? OverloadResolutionAspect.InvocationExpression_Rewrite_InitializerGroupNameExpression(this)
        ?? OverloadResolutionAspect.InvocationExpression_Rewrite_TypeNameExpression(this)
        ?? OverloadResolutionAspect.InvocationExpression_Rewrite_FunctionReferenceExpression(this)
        ?? OverloadResolutionAspect.InvocationExpression_Rewrite_ToUnknown(this);
}
