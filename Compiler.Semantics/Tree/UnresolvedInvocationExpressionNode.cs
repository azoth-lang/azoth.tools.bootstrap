using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UnresolvedInvocationExpressionNode : AmbiguousExpressionNode, IUnresolvedInvocationExpressionNode
{
    protected override bool MayHaveRewrite => true;
    public override IInvocationExpressionSyntax Syntax { get; }
    private RewritableChild<IAmbiguousExpressionNode> expression;
    private bool expressionCached;
    public IAmbiguousExpressionNode TempExpression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public IExpressionNode? Expression => TempExpression as IExpressionNode;
    public IAmbiguousExpressionNode CurrentExpression => expression.UnsafeValue;

    private readonly IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;

    public UnresolvedInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IAmbiguousExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(TempArguments), arguments);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == TempExpression)
            return ContainingLexicalScope;
        if (TempArguments.IndexOf(child) is int argumentIndex)
        {
            if (argumentIndex == 0)
                return TempExpression.FlowLexicalScope().True;

            return TempArguments[argumentIndex - 1].FlowLexicalScope().True;
        }
        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentExpression) return null;
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentExpression) return null;
        return base.InheritedExpectedType(child, descendant, ctx);
    }

    protected override IChildNode Rewrite()
        => OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_FunctionGroupNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_MethodGroupNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_InitializerGroupNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_TypeNameExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_FunctionReferenceExpression(this)
        ?? OverloadResolutionAspect.UnresolvedInvocationExpression_Rewrite_ToUnknown(this)
        ?? base.Rewrite();
}
