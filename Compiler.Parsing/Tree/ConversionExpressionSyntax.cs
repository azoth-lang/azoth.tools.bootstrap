using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ConversionExpressionSyntax : ExpressionSyntax, IConversionExpressionSyntax
{
    public IExpressionSyntax Referent { get; }
    // TODO which conversion operator `as` `as!` or `as?`
    public ITypeSyntax ConvertToType { get; }

    public ConversionExpressionSyntax(
        IExpressionSyntax referent,
        ITypeSyntax convertToType)
        : base(TextSpan.Covering(referent.Span, convertToType.Span))
    {
        Referent = referent;
        ConvertToType = convertToType;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Conversion;

    public override string ToString() =>
        $"{Referent.ToGroupedString(ExpressionPrecedence)} as {ConvertToType}";
}
