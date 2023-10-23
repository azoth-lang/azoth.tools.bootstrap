using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ImplicitNumericConversionExpression : ImplicitConversionExpression, IImplicitNumericConversionExpression
{
    public NumericType ConvertToType { get; }

    public ImplicitNumericConversionExpression(
        TextSpan span,
        ExpressionSemantics semantics,
        IExpression expression,
        NumericType convertToType)
        : base(span, convertToType, semantics, expression)
    {
        ConvertToType = convertToType;
    }

    public override string ToString()
        => $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as {ConvertToType.ToILString()}⟧";
}
