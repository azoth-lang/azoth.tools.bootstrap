using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BoolLiteralExpressionNode : ExpressionNode, IBoolLiteralExpressionNode
{
    public override IBoolLiteralExpressionSyntax Syntax { get; }
    public bool Value => Syntax.Value;
    public BoolConstValueType Type => throw new NotImplementedException();

    public BoolLiteralExpressionNode(IBoolLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
