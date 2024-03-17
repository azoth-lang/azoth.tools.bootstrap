using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ExplicitSimpleTypeConversion : ExplicitConversionExpression, IExplicitSimpleTypeConversionExpression
{
    public SimpleType ConvertToType { get; }

    public ExplicitSimpleTypeConversion(
        TextSpan span,
        IExpression expression,
        bool isOptional,
        SimpleType convertToType)
        : base(span, convertToType.Type, expression, isOptional)
    {
        ConvertToType = convertToType;
    }

    public override string ToString()
    {
        var @operator = IsOptional ? "as?" : "as!";
        return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} {@operator} {ConvertToType}";
    }
}
