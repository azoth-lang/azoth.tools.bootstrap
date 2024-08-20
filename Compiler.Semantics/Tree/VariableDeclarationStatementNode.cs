using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class VariableDeclarationStatementNode : StatementNode, IVariableDeclarationStatementNode
{
    private AttributeLock syncLock;
    public override IVariableDeclarationStatementSyntax Syntax { get; }
    bool IBindingNode.IsLentBinding => false;
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName Name => Syntax.Name;
    public ICapabilityNode? Capability { get; }
    public ITypeNode? Type { get; }
    private RewritableChild<IAmbiguousExpressionNode?> initializer;
    private bool initializerCached;
    public IAmbiguousExpressionNode? Initializer
        => GrammarAttribute.IsCached(in initializerCached) ? initializer.UnsafeValue
            : this.RewritableChild(ref initializerCached, ref initializer);
    public IAmbiguousExpressionNode? CurrentInitializer => initializer.UnsafeValue;
    public IExpressionNode? IntermediateInitializer => Initializer as IExpressionNode;
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                InheritedContainingLexicalScope, ReferenceEqualityComparer.Instance);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public override LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.VariableDeclarationStatement_LexicalScope,
                ReferenceEqualityComparer.Instance);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref syncLock,
                ExpressionTypesAspect.VariableDeclarationStatement_BindingValueId);
    private IMaybeAntetype? bindingAntetype;
    private bool bindingAntetypeCached;
    public IMaybeAntetype BindingAntetype
        => GrammarAttribute.IsCached(in bindingAntetypeCached) ? bindingAntetype!
            : this.Synthetic(ref bindingAntetypeCached, ref bindingAntetype,
                NameBindingAntetypesAspect.VariableDeclarationStatement_BindingAntetype);
    private DataType? bindingType;
    private bool bindingTypeCached;
    public DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, NameBindingTypesAspect.VariableDeclarationStatement_BindingType);
    public override IMaybeAntetype? ResultAntetype => null;
    public override DataType? ResultType => null;
    public override ValueId? ResultValueId => null;
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                NameBindingTypesAspect.VariableDeclarationStatement_FlowStateAfter);
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public override ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.VariableDeclarationStatement_ControlFlowNext);
    private IFixedSet<IDataFlowNode>? dataFlowPrevious;
    private bool dataFlowPreviousCached;
    public IFixedSet<IDataFlowNode> DataFlowPrevious
        => GrammarAttribute.IsCached(in dataFlowPreviousCached) ? dataFlowPrevious!
            : this.Synthetic(ref dataFlowPreviousCached, ref dataFlowPrevious,
                DataFlowAspect.DataFlow_DataFlowPrevious, FixedSet.ObjectEqualityComparer);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyAssigned = Circular.Unset;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned.UnsafeValue
            : this.Circular(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.VariableDeclarationStatement_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.VariableDeclarationStatement_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);

    public VariableDeclarationStatementNode(
        IVariableDeclarationStatementSyntax syntax,
        ICapabilityNode? capability,
        ITypeNode? type,
        IAmbiguousExpressionNode? initializer)
    {
        Syntax = syntax;
        Capability = Child.Attach(this, capability);
        Type = Child.Attach(this, type);
        this.initializer = Child.Create(this, initializer);
    }

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => BindingValueId;

    public IFlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    internal override IMaybeExpressionAntetype? InheritedExpectedAntetype(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentInitializer)
            // The expected antetype for the initializer is the binding antetype since it needs to
            // have any conversion (e.g. to non-const-value type) applied.
            return BindingAntetype;
        return base.InheritedExpectedAntetype(child, descendant, ctx);
    }

    internal override DataType? InheritedExpectedType(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentInitializer)
            // The expected type for the initializer is the binding type since it needs to have any
            // Capability declared on the variable applied.
            return BindingType;
        return base.InheritedExpectedType(child, descendant, ctx);
    }

    internal override bool InheritedImplicitRecoveryAllowed(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (descendant == CurrentInitializer)
            // TODO implicit recovery is allowed for initializers, but only if no variable will be affected
            return true;
        // Deeper descendants should not be affected by the implicit recovery of the initializer
        return false;
    }

    internal override bool InheritedShouldPrepareToReturn(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => false;

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        NameBindingAntetypesAspect.VariableDeclarationStatement_ContributeDiagnostics(this, diagnostics);
        ShadowingAspect.VariableBinding_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
