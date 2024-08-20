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

internal sealed class ForeachExpressionNode : ExpressionNode, IForeachExpressionNode
{
    public override IForeachExpressionSyntax Syntax { get; }
    bool IBindingNode.IsLentBinding => false;
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName VariableName => Syntax.VariableName;
    private RewritableChild<IAmbiguousExpressionNode> inExpression;
    private bool inExpressionCached;
    public IAmbiguousExpressionNode InExpression
        => GrammarAttribute.IsCached(in inExpressionCached) ? inExpression.UnsafeValue
            : this.RewritableChild(ref inExpressionCached, ref inExpression);
    public IAmbiguousExpressionNode CurrentInExpression => inExpression.UnsafeValue;
    public IExpressionNode? IntermediateInExpression => InExpression as IExpressionNode;
    public ITypeNode? DeclaredType { get; }
    private RewritableChild<IBlockExpressionNode> block;
    private bool blockCached;
    public IBlockExpressionNode Block
        => GrammarAttribute.IsCached(in blockCached) ? block.UnsafeValue
            : this.RewritableChild(ref blockCached, ref block);
    public IBlockExpressionNode CurrentBlock => block.UnsafeValue;
    private LexicalScope? containingLexicalScope;
    private bool containingLexicalScopeCached;
    public override LexicalScope ContainingLexicalScope
        => GrammarAttribute.IsCached(in containingLexicalScopeCached) ? containingLexicalScope!
            : this.Inherited(ref containingLexicalScopeCached, ref containingLexicalScope,
                InheritedContainingLexicalScope, ReferenceEqualityComparer.Instance);
    private LexicalScope? lexicalScope;
    private bool lexicalScopeCached;
    public LexicalScope LexicalScope
        => GrammarAttribute.IsCached(in lexicalScopeCached) ? lexicalScope!
            : this.Synthetic(ref lexicalScopeCached, ref lexicalScope,
                LexicalScopingAspect.ForeachExpression_LexicalScope,
                ReferenceEqualityComparer.Instance);
    private ValueAttribute<ITypeDeclarationNode?> referencedIterableDeclaration;
    public ITypeDeclarationNode? ReferencedIterableDeclaration
        => referencedIterableDeclaration.TryGetValue(out var value) ? value
            : referencedIterableDeclaration.GetValue(this, ForeachExpressionAntetypeAspect.ForeachExpression_ReferencedIterableDeclaration);
    private ValueAttribute<IStandardMethodDeclarationNode?> referencedIterateMethod;
    public IStandardMethodDeclarationNode? ReferencedIterateMethod
        => referencedIterateMethod.TryGetValue(out var value) ? value
            : referencedIterateMethod.GetValue(this, ForeachExpressionAntetypeAspect.ForeachExpression_ReferencedIterateMethod);
    private ValueAttribute<IMaybeExpressionAntetype> iteratorAntetype;
    public IMaybeExpressionAntetype IteratorAntetype
        => iteratorAntetype.TryGetValue(out var value) ? value
            : iteratorAntetype.GetValue(this, ForeachExpressionAntetypeAspect.ForeachExpression_IteratorAntetype);
    private DataType? iteratorType;
    private bool iteratorTypeCached;
    public DataType IteratorType
        => GrammarAttribute.IsCached(in iteratorTypeCached) ? iteratorType!
            : this.Synthetic(ref iteratorTypeCached, ref iteratorType, ForeachExpressionTypeAspect.ForeachExpression_IteratorType);
    private ValueAttribute<ITypeDeclarationNode?> referencedIteratorDeclaration;
    public ITypeDeclarationNode? ReferencedIteratorDeclaration
        => referencedIteratorDeclaration.TryGetValue(out var value) ? value
            : referencedIteratorDeclaration.GetValue(this, ForeachExpressionAntetypeAspect.ForeachExpression_ReferencedIteratorDeclaration);
    private ValueAttribute<IStandardMethodDeclarationNode?> referencedNextMethod;
    public IStandardMethodDeclarationNode? ReferencedNextMethod
        => referencedNextMethod.TryGetValue(out var value) ? value
            : referencedNextMethod.GetValue(this, ForeachExpressionAntetypeAspect.ForeachExpression_ReferencedNextMethod);
    private ValueAttribute<IMaybeAntetype> iteratedAntetype;
    public IMaybeAntetype IteratedAntetype
        => iteratedAntetype.TryGetValue(out var value) ? value
            : iteratedAntetype.GetValue(this, ForeachExpressionAntetypeAspect.ForeachExpression_IteratedAntetype);
    private DataType? iteratedType;
    private bool iteratedTypeCached;
    public DataType IteratedType
        => GrammarAttribute.IsCached(in iteratedTypeCached) ? iteratedType!
            : this.Synthetic(ref iteratedTypeCached, ref iteratedType, ForeachExpressionTypeAspect.ForeachExpression_IteratedType);
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref SyncLock,
                ValueIdsAspect.ForeachExpression_BindingValueId);
    private IMaybeAntetype? bindingAntetype;
    private bool bindingAntetypeCached;
    public IMaybeAntetype BindingAntetype
        => GrammarAttribute.IsCached(in bindingAntetypeCached) ? bindingAntetype!
            : this.Synthetic(ref bindingAntetypeCached, ref bindingAntetype,
                NameBindingAntetypesAspect.ForeachExpression_BindingAntetype);
    private DataType? bindingType;
    private bool bindingTypeCached;
    public DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, NameBindingTypesAspect.ForeachExpression_BindingType);
    private Circular<IFlowState> flowStateBeforeBlock = new(IFlowState.Empty);
    private bool flowStateBeforeBlockCached;
    public IFlowState FlowStateBeforeBlock
        => GrammarAttribute.IsCached(in flowStateBeforeBlockCached) ? flowStateBeforeBlock.UnsafeValue
            : this.Circular(ref flowStateBeforeBlockCached, ref flowStateBeforeBlock,
                ForeachExpressionTypeAspect.ForeachExpression_FlowStateBeforeBlock);
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                ExpressionAntetypesAspect.ForeachExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ForeachExpressionTypeAspect.ForeachExpression_Type);
    private Circular<IFlowState> flowStateAfter = new(IFlowState.Empty);
    private bool flowStateAfterCached;
    public override IFlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ForeachExpressionTypeAspect.ForeachExpression_FlowStateAfter);
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
                DefiniteAssignmentAspect.ForeachExpression_DefinitelyAssigned,
                DefiniteAssignmentAspect.DataFlow_DefinitelyAssigned_Initial);
    private Circular<BindingFlags<IVariableBindingNode>> definitelyUnassigned = Circular.Unset;
    private bool definitelyUnassignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached)
            ? definitelyUnassigned.UnsafeValue
            : this.Circular(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.ForeachExpression_DefinitelyUnassigned,
                SingleAssignmentAspect.DataFlow_DefinitelyUnassigned_Initial);

    public ForeachExpressionNode(
        IForeachExpressionSyntax syntax,
        IAmbiguousExpressionNode inExpression,
        ITypeNode? type,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.inExpression = Child.Create(this, inExpression);
        DeclaredType = Child.Attach(this, type);
        this.block = Child.Create(this, block);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
        => child == Block ? LexicalScope : ContainingLexicalScope;

    public PackageNameScope PackageNameScope() => InheritedPackageNameScope();

    internal override IFlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == Block)
            return FlowStateBeforeBlock;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx)
        // Include the BindingValueId in the value id flow
        => BindingValueId;

    protected override void CollectDiagnostics(DiagnosticCollectionBuilder diagnostics)
    {
        ForeachExpressionTypeAspect.ForeachExpression_ContributeDiagnostics(this, diagnostics);
        ShadowingAspect.VariableBinding_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    protected override ControlFlowSet ComputeControlFlowNext()
        => ControlFlowAspect.ForeachExpression_ControlFlowNext(this);

    internal override ControlFlowSet InheritedControlFlowFollowing(IChildNode child, IChildNode descendant, IInheritanceContext ctx)
    {
        if (child == CurrentInExpression)
            return ControlFlowSet.CreateNormal(Block);
        if (child == CurrentBlock)
            // Technically, `next()` is called on the iterator before the block is looped. But there
            // is no node that corresponds to that and it has no effect on the control flow.
            return ControlFlowSet.CreateLoop(Block).Union(ControlFlowFollowing());
        return base.InheritedControlFlowFollowing(child, descendant, ctx);
    }
}
