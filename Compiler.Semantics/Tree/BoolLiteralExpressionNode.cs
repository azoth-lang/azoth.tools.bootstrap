using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BoolLiteralExpressionNode : LiteralExpressionNode, IBoolLiteralExpressionNode
{
    public override IBoolLiteralExpressionSyntax Syntax { get; }
    public bool Value => Syntax.Value;
    private IMaybeExpressionAntetype? antetype;
    private bool antetypeCached;
    public override IMaybeExpressionAntetype Antetype
        => GrammarAttribute.IsCached(in antetypeCached) ? antetype!
            : this.Synthetic(ref antetypeCached, ref antetype,
                TypeExpressionsAntetypesAspect.BoolLiteralExpression_NamedAntetype);
    public override BoolConstValueType Type => ExpressionTypesAspect.BoolLiteralExpression_Type(this);

    public BoolLiteralExpressionNode(IBoolLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
