using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
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
    private ValueAttribute<IMaybeAntetype> bindingAntetype;
    public IMaybeAntetype BindingAntetype
        => bindingAntetype.TryGetValue(out var value) ? value
            : bindingAntetype.GetValue(this, NameBindingAntetypesAspect.ForeachExpression_BindingAntetype);
    public DataType BindingType => throw new NotImplementedException();
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.ForeachExpression_Antetype);

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
}
