using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NewObjectExpressionNode : ExpressionNode, INewObjectExpressionNode
{
    public override INewObjectExpressionSyntax Syntax { get; }
    public ITypeNameNode ConstructingType { get; }
    public IdentifierName? ConstructorName => Syntax.ConstructorName;
    private readonly IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> TempArguments => arguments;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IEnumerable<IAmbiguousExpressionNode> TempAllArguments => TempArguments;
    public IFixedList<IExpressionNode?> Arguments => arguments.AsFinalType;
    public IEnumerable<IExpressionNode?> AllArguments => Arguments;
    private IMaybeAntetype? constructingAntetype;
    private bool constructingAntetypeCached;
    public IMaybeAntetype ConstructingAntetype
        => GrammarAttribute.IsCached(in constructingAntetypeCached) ? constructingAntetype!
            : this.Synthetic(ref constructingAntetypeCached, ref constructingAntetype,
                NameBindingAntetypesAspect.NewObjectExpression_ConstructingAntetype);
    private IFixedSet<IConstructorDeclarationNode>? referencedConstructors;
    private bool referencedConstructorsCached;
    public IFixedSet<IConstructorDeclarationNode> ReferencedConstructors
        => GrammarAttribute.IsCached(in referencedConstructorsCached) ? referencedConstructors!
            : this.Synthetic(ref referencedConstructorsCached, ref referencedConstructors,
                BindingNamesAspect.NewObjectExpression_ReferencedConstructors,
                FixedSet.ObjectEqualityComparer);
    private IFixedSet<IConstructorDeclarationNode>? compatibleConstructors;
    private bool compatibleConstructorsCached;
    public IFixedSet<IConstructorDeclarationNode> CompatibleConstructors
        => GrammarAttribute.IsCached(in compatibleConstructorsCached) ? compatibleConstructors!
            : this.Synthetic(ref compatibleConstructorsCached, ref compatibleConstructors,
                OverloadResolutionAspect.NewObjectExpression_CompatibleConstructors,
                FixedSet.ObjectEqualityComparer);
    private IConstructorDeclarationNode? referencedConstructor;
    private bool referencedConstructorCached;
    public IConstructorDeclarationNode? ReferencedConstructor
        => GrammarAttribute.IsCached(in referencedConstructorCached) ? referencedConstructor!
            : this.Synthetic(ref referencedConstructorCached, ref referencedConstructor,
                OverloadResolutionAspect.NewObjectExpression_ReferencedConstructor,
                ReferenceEqualityComparer.Instance);
    private ContextualizedOverload? contextualizedOverload;
    private bool contextualizedOverloadCached;
    public ContextualizedOverload? ContextualizedOverload
        => GrammarAttribute.IsCached(in contextualizedOverloadCached) ? contextualizedOverload
            : this.Synthetic(ref contextualizedOverloadCached, ref contextualizedOverload,
                ExpressionTypesAspect.NewObjectExpression_ContextualizedOverload);
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.NewObjectExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type,
                ExpressionTypesAspect.NewObjectExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.NewObjectExpression_FlowStateAfter);

    public NewObjectExpressionNode(
        INewObjectExpressionSyntax syntax,
        ITypeNameNode type,
        IEnumerable<IAmbiguousExpressionNode> arguments)
    {
        Syntax = syntax;
        ConstructingType = Child.Attach(this, type);
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(TempArguments), arguments);
    }

    internal override void CollectContributors_Diagnostics(List<SemanticNode> contributors)
    {
        contributors.Add(this);
        base.CollectContributors_Diagnostics(contributors);
    }

    internal override void Contribute_Diagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        OverloadResolutionAspect.NewObjectExpression_Contribute_Diagnostics(this, diagnostics);
        ExpressionTypesAspect.NewObjectExpression_ContributeDiagnostics(this, diagnostics);
        base.Contribute_Diagnostics(diagnostics);
    }

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (child == ConstructingType)
            return ContainingLexicalScope;
        var argumentIndex = TempArguments.IndexOf((IChildNode)child)
                            ?? throw new ArgumentException("Is not a child of this node.", nameof(child));
        if (argumentIndex == 0)
            return ContainingLexicalScope;

        return TempArguments[argumentIndex - 1].FlowLexicalScope().True;
    }

    public PackageNameScope PackageNameScope()
        => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext());

    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    internal override IFlowState Inherited_FlowStateBefore(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && arguments.Current.IndexOf(ambiguousExpression) is int index and > 0)
            return Arguments[index - 1]?.FlowStateAfter ?? IFlowState.Empty;
        return base.Inherited_FlowStateBefore(child, descendant, ctx);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.NewObjectExpression_ControlFlowNext(this);

    internal override ControlFlowSet Inherited_ControlFlowFollowing(
        SemanticNode child,
        SemanticNode descendant,
        IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index && index < CurrentArguments.Count - 1)
            return ControlFlowSet.CreateNormal(Arguments[index + 1]);
        return base.Inherited_ControlFlowFollowing(child, descendant, ctx);
    }

    internal override IMaybeExpressionAntetype? Inherited_ExpectedAntetype(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index)
            // TODO it would be better if this didn't depend on types, but only on antetypes
            return ContextualizedOverload?.ParameterTypes[index].Type.ToAntetype();
        return base.Inherited_ExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? Inherited_ExpectedType(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        if (descendant is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index)
            return ContextualizedOverload?.ParameterTypes[index].Type;
        return base.Inherited_ExpectedType(child, descendant, ctx);
    }
}
