using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ImplicitSimpleTypeConversionExpression : ImplicitConversionExpression, IImplicitSimpleTypeConversionExpression
{
    public SimpleType ConvertToType { get; }

    public ImplicitSimpleTypeConversionExpression(TextSpan span, IExpression expression, SimpleType convertToType)
        : base(span, convertToType.Type, expression)
    {
        ConvertToType = convertToType;
    }

    public override string ToString()
        => $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as {ConvertToType}⟧";
}
