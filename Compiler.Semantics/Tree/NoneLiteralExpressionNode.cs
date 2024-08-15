using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NoneLiteralExpressionNode : LiteralExpressionNode, INoneLiteralExpressionNode
{
    public override INoneLiteralExpressionSyntax Syntax { get; }
    public override IMaybeExpressionAntetype Antetype
        => ExpressionAntetypesAspect.NoneLiteralExpression_Antetype(this);
    public override OptionalType Type => ExpressionTypesAspect.NoneLiteralExpression_Type(this);

    public NoneLiteralExpressionNode(INoneLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
