using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StringLiteralExpressionNode : ExpressionNode, IStringLiteralExpressionNode
{
    public override IStringLiteralExpressionSyntax Syntax { get; }
    public string Value => Syntax.Value;
    public DataType Type => throw new System.NotImplementedException();

    public StringLiteralExpressionNode(IStringLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
