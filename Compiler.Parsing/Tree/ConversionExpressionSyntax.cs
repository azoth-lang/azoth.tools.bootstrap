using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ConversionExpressionSyntax : DataTypedExpressionSyntax, IConversionExpressionSyntax
{
    public IExpressionSyntax Referent { get; }
    public ConversionOperator Operator { get; }
    public ITypeSyntax ConvertToType { get; }

    public ConversionExpressionSyntax(
        IExpressionSyntax referent,
        ConversionOperator @operator,
        ITypeSyntax convertToType)
        : base(TextSpan.Covering(referent.Span, convertToType.Span))
    {
        Referent = referent;
        ConvertToType = convertToType;
        Operator = @operator;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Conversion;

    public override string ToString()
        => $"{Referent.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {ConvertToType}";
}
