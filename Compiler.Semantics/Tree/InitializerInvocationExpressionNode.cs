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

internal sealed class InitializerInvocationExpressionNode : ExpressionNode, IInitializerInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IInitializerGroupNameNode InitializerGroup { get; }
    private readonly IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IFixedList<IExpressionNode?> IntermediateArguments => arguments.Intermediate;
    private ValueAttribute<IFixedSet<IInitializerDeclarationNode>> compatibleDeclarations;
    public IFixedSet<IInitializerDeclarationNode> CompatibleDeclarations
        => compatibleDeclarations.TryGetValue(out var value) ? value
            : compatibleDeclarations.GetValue(this, OverloadResolutionAspect.InitializerInvocationExpression_CompatibleDeclarations);
    private ValueAttribute<IInitializerDeclarationNode?> referencedDeclaration;
    public IInitializerDeclarationNode? ReferencedDeclaration
        => referencedDeclaration.TryGetValue(out var value) ? value
            : referencedDeclaration.GetValue(this, OverloadResolutionAspect.InitializerInvocationExpression_ReferencedDeclaration);
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.InitializerInvocationExpression_Antetype);
    private ValueAttribute<ContextualizedOverload?> contextualizedOverload;
    public ContextualizedOverload? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this, ExpressionTypesAspect.InitializerInvocationExpression_ContextualizedOverload);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.InitializerInvocationExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.InitializerInvocationExpression_FlowStateAfter);

    public InitializerInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IInitializerGroupNameNode initializerGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        InitializerGroup = Child.Attach(this, initializerGroup);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    public IFlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && arguments.Current.IndexOf(ambiguousExpression) is int index and > 0)
            return IntermediateArguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }
}
