using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
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
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IFixedList<IExpressionNode?> IntermediateArguments => arguments.Intermediate;
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
    private ValueAttribute<ContextualizedOverload?> contextualizedOverload;
    public ContextualizedOverload? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this, ExpressionTypesAspect.FunctionInvocationExpression_ContextualizedOverload);
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
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (Arguments.IndexOf(child) is int argumentIndex)
        {
            if (argumentIndex == 0) return GetContainingLexicalScope();

            return Arguments[argumentIndex - 1].GetFlowLexicalScope().True;
        }

        return base.InheritedContainingLexicalScope(child, descendant, ctx);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        OverloadResolutionAspect.FunctionInvocationExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

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

    public IFlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
}
