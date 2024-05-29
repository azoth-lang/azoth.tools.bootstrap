using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ForeachExpressionNode : ExpressionNode, IForeachExpressionNode
{
    public override IForeachExpressionSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName VariableName => Syntax.VariableName;
    private Child<IAmbiguousExpressionNode> inExpression;
    public IAmbiguousExpressionNode InExpression => inExpression.Value;
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

    internal override IFlowNode InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (child == InExpression)
            return base.InheritedPredecessor(child, descendant);
        if (child == Block)
            return (IFlowNode)InExpression;

        return base.InheritedPredecessor(child, descendant);
    }
}
