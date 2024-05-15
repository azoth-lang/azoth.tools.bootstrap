using System;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NoneLiteralExpressionNode : ExpressionNode, INoneLiteralExpressionNode
{
    public override INoneLiteralExpressionSyntax Syntax { get; }
    public OptionalType Type => throw new NotImplementedException();

    public NoneLiteralExpressionNode(INoneLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
