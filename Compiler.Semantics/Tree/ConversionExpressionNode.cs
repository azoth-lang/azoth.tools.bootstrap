using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ConversionExpressionNode : ExpressionNode, IConversionExpressionNode
{
    public override IConversionExpressionSyntax Syntax { get; }
    public IUntypedExpressionNode Referent { get; }
    public ConversionOperator Operator => Syntax.Operator;
    public ITypeNode ConvertToType { get; }

    public ConversionExpressionNode(
        IConversionExpressionSyntax syntax,
        IUntypedExpressionNode referent,
        ITypeNode convertToType)
    {
        Syntax = syntax;
        Referent = Child.Attach(this, referent);
        ConvertToType = Child.Attach(this, convertToType);
    }
}
