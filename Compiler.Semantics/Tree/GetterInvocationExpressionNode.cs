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

internal sealed class GetterInvocationExpressionNode : ExpressionNode, IGetterInvocationExpressionNode
{
    public override IMemberAccessExpressionSyntax Syntax { get; }
    private RewritableChild<IExpressionNode> context;
    private bool contextCached;
    public IExpressionNode Context
        => GrammarAttribute.IsCached(in contextCached) ? context.UnsafeValue
            : this.RewritableChild(ref contextCached, ref context);
    public IExpressionNode CurrentContext => context.UnsafeValue;
    public StandardName PropertyName { get; }
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments => Context.Yield();
    public IEnumerable<IExpressionNode?> AllArguments => Context.Yield();
    public IFixedSet<IPropertyAccessorDeclarationNode> ReferencedPropertyAccessors { get; }
    public IGetterMethodDeclarationNode? ReferencedDeclaration { get; }
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.GetterInvocationExpression_Antetype);
    private ContextualizedOverload? contextualizedOverload;
    private bool contextualizedOverloadCached;
    public ContextualizedOverload? ContextualizedOverload
        => GrammarAttribute.IsCached(in contextualizedOverloadCached) ? contextualizedOverload
            : this.Synthetic(ref contextualizedOverloadCached, ref contextualizedOverload,
                ExpressionTypesAspect.GetterInvocationExpression_ContextualizedOverload);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.GetterInvocationExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.GetterInvocationExpression_FlowStateAfter);

    public GetterInvocationExpressionNode(
        IMemberAccessExpressionSyntax syntax,
        IExpressionNode context,
        StandardName propertyName,
        IFixedSet<IPropertyAccessorDeclarationNode> referencedPropertyAccessors,
        IGetterMethodDeclarationNode? referencedDeclaration)
    {
        Syntax = syntax;
        this.context = Child.Create(this, context);
        PropertyName = propertyName;
        ReferencedPropertyAccessors = referencedPropertyAccessors.ToFixedSet();
        ReferencedDeclaration = referencedDeclaration;
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.GetterInvocationExpression_ControlFlowNext(this);

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentContext)
            // TODO it would be better if this didn't depend on types, but only on antetypes
            return ContextualizedOverload?.SelfParameterType?.Type.ToUpperBound().ToAntetype();
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentContext)
            return ContextualizedOverload?.SelfParameterType?.Type.ToUpperBound();
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics, bool contributeAttribute = true)
    {
        ExpressionTypesAspect.GetterInvocationExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics, contributeAttribute);
    }
}
