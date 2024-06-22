using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ForeachExpressionNode : ExpressionNode, IForeachExpressionNode
{
    public override IForeachExpressionSyntax Syntax { get; }
    bool IBindingNode.IsLentBinding => false;
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName VariableName => Syntax.VariableName;
    private Child<IAmbiguousExpressionNode> inExpression;
    public IAmbiguousExpressionNode InExpression => inExpression.Value;
    public IExpressionNode FinalInExpression => (IExpressionNode)inExpression.FinalValue;
    public ITypeNode? DeclaredType { get; }
    public IBlockExpressionNode Block { get; }
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);
    private ValueAttribute<LexicalScope> lexicalScope;
    public LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopingAspect.ForeachExpression_LexicalScope);
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
    private ValueAttribute<IMaybeAntetype> bindingAntetype;
    public IMaybeAntetype BindingAntetype
        => bindingAntetype.TryGetValue(out var value) ? value
            : bindingAntetype.GetValue(this, NameBindingAntetypesAspect.ForeachExpression_BindingAntetype);
    private DataType? bindingType;
    private bool bindingTypeCached;
    public DataType BindingType
        => GrammarAttribute.IsCached(in bindingTypeCached) ? bindingType!
            : this.Synthetic(ref bindingTypeCached, ref bindingType, NameBindingTypesAspect.ForeachExpression_BindingType);
    private Circular<FlowState> flowStateBeforeBlock = new(FlowState.Empty);
    private bool flowStateBeforeBlockCached;
    public FlowState FlowStateBeforeBlock
        => GrammarAttribute.IsCached(in flowStateBeforeBlockCached) ? flowStateBeforeBlock.UnsafeValue
            : this.Circular(ref flowStateBeforeBlockCached, ref flowStateBeforeBlock,
                ForeachExpressionTypeAspect.ForeachExpression_FlowStateBeforeBlock);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.ForeachExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ForeachExpressionTypeAspect.ForeachExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ForeachExpressionTypeAspect.ForeachExpression_FlowStateAfter);

    public ForeachExpressionNode(
        IForeachExpressionSyntax syntax,
        IAmbiguousExpressionNode inExpression,
        ITypeNode? type,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        this.inExpression = Child.Create(this, inExpression);
        DeclaredType = Child.Attach(this, type);
        Block = Child.Attach(this, block);
    }

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode child, IChildNode descendant)
        => child == Block ? LexicalScope : ContainingLexicalScope;

    public new PackageNameScope InheritedPackageNameScope() => base.InheritedPackageNameScope();

    internal override FlowState InheritedFlowStateBefore(
        IChildNode child,
        IChildNode descendant,
        IInheritanceContext ctx)
    {
        if (child == Block)
            return FlowStateBeforeBlock;
        return base.InheritedFlowStateBefore(child, descendant, ctx);
    }
}
