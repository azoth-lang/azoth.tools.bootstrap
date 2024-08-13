using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MethodInvocationExpressionNode : ExpressionNode, IMethodInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    public IMethodGroupNameNode MethodGroup { get; }
    private readonly IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IFixedList<IExpressionNode?> IntermediateArguments => arguments.Intermediate;
    private IFixedSet<IStandardMethodDeclarationNode>? compatibleDeclarations;
    private bool compatibleDeclarationsCached;
    public IFixedSet<IStandardMethodDeclarationNode> CompatibleDeclarations
        => GrammarAttribute.IsCached(in compatibleDeclarationsCached) ? compatibleDeclarations!
            : this.Synthetic(ref compatibleDeclarationsCached, ref compatibleDeclarations,
                OverloadResolutionAspect.MethodInvocationExpression_CompatibleDeclarations,
                FixedSet.ObjectEqualityComparer);
    private IStandardMethodDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public IStandardMethodDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration!
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                OverloadResolutionAspect.MethodInvocationExpression_ReferencedDeclaration,
                ReferenceEqualityComparer.Instance);
    private ContextualizedOverload? contextualizedOverload;
    private bool contextualizedOverloadCached;
    public ContextualizedOverload? ContextualizedOverload
        => GrammarAttribute.IsCached(in contextualizedOverloadCached) ? contextualizedOverload
            : this.Synthetic(ref contextualizedOverloadCached, ref contextualizedOverload,
                ExpressionTypesAspect.MethodInvocationExpression_ContextualizedOverload);
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.MethodInvocationExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.MethodInvocationExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.MethodInvocationExpression_FlowStateAfter);

    public MethodInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IMethodGroupNameNode methodGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        MethodGroup = Child.Attach(this, methodGroup);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index)
        {
            if (index == 0)
                return MethodGroup.FlowStateAfter;
            return IntermediateArguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        }
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.MethodInvocationExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == MethodGroup)
        {
            if (!Arguments.IsEmpty)
                return ControlFlowSet.CreateNormal(IntermediateArguments[0]);
        }
        else if (child is IAmbiguousExpressionNode ambiguousExpression
                 && CurrentArguments.IndexOf(ambiguousExpression) is int index && index < CurrentArguments.Count - 1)
            return ControlFlowSet.CreateNormal(IntermediateArguments[index + 1]);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }

    internal override bool InheritedImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        // TODO this is a hack that is working only because method group nodes aren't getting the default expression InheritedImplicitRecoveryAllowed applied
        if (child == MethodGroup && descendant == MethodGroup.CurrentContext)
            return true;
        return base.InheritedImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == MethodGroup.CurrentContext)
            // TODO it would be better if this didn't depend on types, but only on antetypes
            return ContextualizedOverload?.SelfParameterType?.Type.ToUpperBound().ToAntetype();
        if (descendant is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index)
            // TODO it would be better if this didn't depend on types, but only on antetypes
            return ContextualizedOverload?.ParameterTypes[index].Type.ToAntetype();
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == MethodGroup.CurrentContext)
            return ContextualizedOverload?.SelfParameterType?.Type.ToUpperBound();
        if (descendant is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index)
            return ContextualizedOverload?.ParameterTypes[index].Type;
        return base.InheritedExpectedType(child, descendant, ctx);
    }

    protected override void CollectDiagnostics(DiagnosticsBuilder diagnostics)
    {
        OverloadResolutionAspect.MethodInvocationExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
