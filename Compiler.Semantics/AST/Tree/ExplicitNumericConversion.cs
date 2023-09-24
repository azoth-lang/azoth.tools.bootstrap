using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ExplicitNumericConversion : ExplicitConversionExpression, IExplicitNumericConversionExpression
{
    public NumericType ConvertToType { get; }

    public ExplicitNumericConversion(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression expression,
        bool isOptional,
        NumericType convertToType)
        : base(span, dataType, semantics, expression, isOptional)
    {
        ConvertToType = convertToType;
    }

    public override string ToString()
    {
        var @operator = IsOptional ? "as?" : "as!";
        return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} {@operator} {ConvertToType.ToILString()}";
    }
}
