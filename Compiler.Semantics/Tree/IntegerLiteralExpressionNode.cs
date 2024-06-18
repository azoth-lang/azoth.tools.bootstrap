using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IntegerLiteralExpressionNode : LiteralExpressionNode, IIntegerLiteralExpressionNode
{
    public override IIntegerLiteralExpressionSyntax Syntax { get; }
    public BigInteger Value => Syntax.Value;
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, TypeExpressionsAntetypesAspect.IntegerLiteralExpression_NamedAntetype);
    private IntegerConstValueType? type;
    private bool typeCached;
    public override IntegerConstValueType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.IntegerLiteralExpression_Type, ref type);

    public IntegerLiteralExpressionNode(IIntegerLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
