using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ImplicitOptionalConversionExpression : ImplicitConversionExpression, IImplicitOptionalConversionExpression
{
    public OptionalType ConvertToType { get; }

    public ImplicitOptionalConversionExpression(
        TextSpan span,
        DataType dataType,
        IExpression expression,
        OptionalType convertToType)
        : base(span, dataType, expression)
    {
        ConvertToType = convertToType;
    }

    public override string ToString()
        => $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as {ConvertToType.ToILString()}⟧";
}
