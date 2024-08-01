using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionReferenceInvocationExpressionNode : ExpressionNode, IFunctionReferenceInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> expression;
    private bool expressionCached;
    public IExpressionNode Expression
        => GrammarAttribute.IsCached(in expressionCached) ? expression.UnsafeValue
            : this.RewritableChild(ref expressionCached, ref expression);
    public FunctionAntetype FunctionAntetype { get; }
    private readonly IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IFixedList<IExpressionNode?> IntermediateArguments => arguments.Intermediate;
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.FunctionReferenceInvocation_Antetype);
    private FunctionType? functionType;
    private bool functionTypeCached;
    public FunctionType FunctionType
        => GrammarAttribute.IsCached(in functionTypeCached) ? functionType!
            : this.Synthetic(ref functionTypeCached, ref functionType, ExpressionTypesAspect.FunctionReferenceInvocation_FunctionType);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.FunctionReferenceInvocation_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FunctionReferenceInvocation_FlowStateAfter);

    public FunctionReferenceInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IExpressionNode expression,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
        FunctionAntetype = (FunctionAntetype)expression.Antetype;
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override IFlowState InheritedFlowStateBefore(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && arguments.Current.IndexOf(ambiguousExpression) is int index and > 0)
            return IntermediateArguments[index - 1]?.FlowStateAfter ?? Expression.FlowStateAfter;

        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }
}
