using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionInvocationExpressionNode : ExpressionNode, IFunctionInvocationExpressionNode
{
    public override IInvocationExpressionSyntax Syntax { get; }
    private RewritableChild<IFunctionGroupNameNode> functionGroup;
    private bool functionGroupCached;
    public IFunctionGroupNameNode FunctionGroup
        => GrammarAttribute.IsCached(in functionGroupCached) ? functionGroup.UnsafeValue
            : this.RewritableChild(ref functionGroupCached, ref functionGroup);
    private readonly IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments => TempArguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IEnumerable<IExpressionNode?> AllArguments => Arguments;
    private IFixedSet<IFunctionLikeDeclarationNode>? compatibleDeclarations;
    private bool compatibleDeclarationsCached;
    public IFixedSet<IFunctionLikeDeclarationNode> CompatibleDeclarations
        => GrammarAttribute.IsCached(in compatibleDeclarationsCached) ? compatibleDeclarations!
            : this.Synthetic(ref compatibleDeclarationsCached, ref compatibleDeclarations,
                OverloadResolutionAspect.FunctionInvocationExpression_CompatibleDeclarations,
                FixedSet.ObjectEqualityComparer);
    private IFunctionLikeDeclarationNode? referencedDeclaration;
    private bool referencedDeclarationCached;
    public IFunctionLikeDeclarationNode? ReferencedDeclaration
        => GrammarAttribute.IsCached(in referencedDeclarationCached) ? referencedDeclaration
            : this.Synthetic(ref referencedDeclarationCached, ref referencedDeclaration,
                OverloadResolutionAspect.FunctionInvocationExpression_ReferencedDeclaration,
                ReferenceEqualityComparer.Instance);
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.FunctionInvocationExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.FunctionInvocationExpression_Type);
    private ContextualizedOverload? contextualizedOverload;
    private bool contextualizedOverloadCached;
    public ContextualizedOverload? ContextualizedOverload
        => GrammarAttribute.IsCached(in contextualizedOverloadCached) ? contextualizedOverload
            : this.Synthetic(ref contextualizedOverloadCached, ref contextualizedOverload,
                ExpressionTypesAspect.FunctionInvocationExpression_ContextualizedOverload);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.FunctionInvocationExpression_FlowStateAfter);

    public FunctionInvocationExpressionNode(
        IInvocationExpressionSyntax syntax,
        IFunctionGroupNameNode functionGroup,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        this.functionGroup = Child.Create(this, functionGroup);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(TempArguments), arguments);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (TempArguments.IndexOf(child) is int argumentIndex)
        {
            if (argumentIndex == 0) return ContainingLexicalScope;

            return TempArguments[argumentIndex - 1].FlowLexicalScope().True;
        }

        return base.Inherited_ContainingLexicalScope(child, descendant, ctx);
    }

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        OverloadResolutionAspect.FunctionInvocationExpression_ContributeDiagnostics(this, diagnostics);
        ExpressionTypesAspect.FunctionInvocationExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    internal override IFlowState Inherited_FlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index and > 0)
            return Arguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;

        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.FunctionInvocationExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == FunctionGroup)
        {
            if (!TempArguments.IsEmpty)
                return ControlFlowSet.CreateNormal(Arguments[0]);
        }
        else if (child is IAmbiguousExpressionNode ambiguousExpression
                 && CurrentArguments.IndexOf(ambiguousExpression) is int index && index < CurrentArguments.Count - 1)
            return ControlFlowSet.CreateNormal(Arguments[index + 1]);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index)
            // TODO it would be better if this didn't depend on types, but only on antetypes
            return ContextualizedOverload?.ParameterTypes[index].Type.ToAntetype();
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index)
            return ContextualizedOverload?.ParameterTypes[index].Type;
        return base.InheritedExpectedType(child, descendant, ctx);
    }
}
