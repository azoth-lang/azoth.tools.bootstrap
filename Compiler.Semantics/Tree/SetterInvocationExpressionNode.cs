using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SetterInvocationExpressionNode : ExpressionNode, ISetterInvocationExpressionNode
{
    public override IAssignmentExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public StandardName PropertyName { get; }
    private RewritableChild<IAmbiguousExpressionNode> value;
    private bool valueCached;
    public IAmbiguousExpressionNode TempValue
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IAmbiguousExpressionNode CurrentValue => value.UnsafeValue;
    public IExpressionNode? Value => TempValue as IExpressionNode;
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments => [Context, TempValue];
    public IEnumerable<IExpressionNode?> AllArguments => [Context, Value];
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    public ISetterMethodDeclarationNode? ReferencedDeclaration { get; }
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.SetterInvocationExpression_Antetype);
    private ContextualizedOverload? contextualizedOverload;
    private bool contextualizedOverloadCached;
    public ContextualizedOverload? ContextualizedOverload
        => GrammarAttribute.IsCached(in contextualizedOverloadCached) ? contextualizedOverload
            : this.Synthetic(ref contextualizedOverloadCached, ref contextualizedOverload,
                ExpressionTypesAspect.SetterInvocationExpression_ContextualizedOverload);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.SetterInvocationExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.SetterInvocationExpression_FlowStateAfter);

    public SetterInvocationExpressionNode(
        IAssignmentExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IAmbiguousExpressionNode value,
        IEnumerable<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        ISetterMethodDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        PropertyName = propertyName;
        this.value = Child.Create(this, value);
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
    }

    internal override IFlowState Inherited_FlowStateBefore(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (child == CurrentValue) return Context.FlowStateAfter;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.SetterInvocationExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == Context) return ControlFlowSet.CreateNormal(Value);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentContext)
            return ContextualizedOverload?.SelfParameterType?.Type.ToUpperBound().ToAntetype();
        if (descendant == CurrentValue)
            return ContextualizedOverload?.ParameterTypes[0].Type.ToAntetype();
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentContext)
            return ContextualizedOverload?.SelfParameterType?.Type.ToUpperBound();
        if (descendant == CurrentValue)
            return ContextualizedOverload?.ParameterTypes[0].Type;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ExpressionTypesAspect.SetterInvocationExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }
}
