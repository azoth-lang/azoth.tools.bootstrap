using Azoth.Tools.Bootstrap.Compiler.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    /// <summary>
    /// A non-missing token
    /// </summary>
    [Closed(
        typeof(IEssentialToken),
        typeof(ITriviaToken))]
    public partial interface IToken
    {
        TextSpan Span { get; }

        string Text(CodeText code);
    }
}
