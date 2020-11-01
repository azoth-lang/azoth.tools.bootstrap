using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    [Closed(
        typeof(ITrueKeywordToken),
        typeof(IFalseKeywordToken))]
    public partial interface IBooleanLiteralToken : IKeywordToken
    {
        bool Value { get; }
    }

    public partial interface ITrueKeywordToken : IBooleanLiteralToken { }
    public partial interface IFalseKeywordToken : IBooleanLiteralToken { }
}
