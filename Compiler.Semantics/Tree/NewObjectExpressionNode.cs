using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NewObjectExpressionNode : ExpressionNode, INewObjectExpressionNode
{
    public override INewObjectExpressionSyntax Syntax { get; }
    public ITypeNameNode ConstructingType { get; }
    public IdentifierName? ConstructorName => Syntax.ConstructorName;
    private readonly IRewritableChildList<IAmbiguousExpressionNode, IExpressionNode> arguments;
    public IFixedList<IAmbiguousExpressionNode> Arguments => arguments;
    public IFixedList<IAmbiguousExpressionNode> CurrentArguments => arguments.Current;
    public IFixedList<IExpressionNode?> IntermediateArguments => arguments.Intermediate;
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
    private ValueAttribute<ContextualizedOverload?> contextualizedOverload;
    public ContextualizedOverload? ContextualizedOverload
        => contextualizedOverload.TryGetValue(out var value) ? value
            : contextualizedOverload.GetValue(this,
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
        this.arguments = ChildList<IExpressionNode>.Create(this, nameof(Arguments), arguments);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        OverloadResolutionAspect.NewObjectExpression_ContributeDiagnostics(this, diagnostics);
        ExpressionTypesAspect.NewObjectExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == ConstructingType)
            return GetContainingLexicalScope();
        var argumentIndex = Arguments.IndexOf(child)
                            ?? throw new ArgumentException("Is not a child of this node.", nameof(child));
        if (argumentIndex == 0)
            return GetContainingLexicalScope();

        return Arguments[argumentIndex - 1].GetFlowLexicalScope().True;
    }

    public new PackageNameScope InheritedPackageNameScope() => base.InheritedPackageNameScope();

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

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.NewObjectExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child is IAmbiguousExpressionNode ambiguousExpression
            && CurrentArguments.IndexOf(ambiguousExpression) is int index && index < CurrentArguments.Count - 1)
            return ControlFlowSet.CreateNormal(IntermediateArguments[index + 1]);
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }
}
