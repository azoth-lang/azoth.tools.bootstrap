using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
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
    public StandardName PropertyName { get; }
    private RewritableChild<IAmbiguousExpressionNode> value;
    private bool valueCached;
    public IAmbiguousExpressionNode Value
        => GrammarAttribute.IsCached(in valueCached) ? value.UnsafeValue
            : this.RewritableChild(ref valueCached, ref value);
    public IAmbiguousExpressionNode CurrentValue => value.UnsafeValue;
    public IExpressionNode? IntermediateValue => Value as IExpressionNode;
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

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == CurrentValue) return Context.FlowStateAfter;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.SetterInvocationExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == Context) return ControlFlowSet.CreateNormal(IntermediateValue);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }
}
