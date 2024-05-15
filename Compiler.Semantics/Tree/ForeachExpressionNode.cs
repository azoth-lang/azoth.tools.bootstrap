using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ForeachExpressionNode : ExpressionNode, IForeachExpressionNode
{
    public override IForeachExpressionSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName VariableName => Syntax.VariableName;
    public IUntypedExpressionNode InExpression { get; }
    public ITypeNode? Type { get; }
    public IBlockExpressionNode Block { get; }

    public ForeachExpressionNode(
        IForeachExpressionSyntax syntax,
        IUntypedExpressionNode inExpression,
        ITypeNode? type,
        IBlockExpressionNode block)
    {
        Syntax = syntax;
        InExpression = Child.Attach(this, inExpression);
        Type = Child.Attach(this, type);
        Block = Child.Attach(this, block);
    }
}
