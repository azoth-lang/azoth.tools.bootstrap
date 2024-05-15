using System;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IntegerLiteralExpressionNode : ExpressionNode, IIntegerLiteralExpressionNode
{
    public override IIntegerLiteralExpressionSyntax Syntax { get; }
    public BigInteger Value => Syntax.Value;
    public IntegerConstValueType Type => throw new NotImplementedException();

    public IntegerLiteralExpressionNode(IIntegerLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
