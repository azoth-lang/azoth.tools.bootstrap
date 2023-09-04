using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ITriviaToken = Azoth.Tools.Bootstrap.Compiler.Tokens.ITriviaToken;

namespace Azoth.Tools.Bootstrap.Compiler.Lexing;

public static class TokenIteratorExtensions
{
    public static ITokenIterator<IEssentialToken> WhereNotTrivia(this ITokenIterator<IToken> tokens)
    {
        return new WhereNotTriviaIterator(tokens);
    }

    private class WhereNotTriviaIterator : ITokenIterator<IEssentialToken>
    {
        private readonly ITokenIterator<IToken> tokens;

        public WhereNotTriviaIterator(ITokenIterator<IToken> tokens)
        {
            this.tokens = tokens;
            if (tokens.Current is ITriviaToken)
                Next();
        }

        public ParseContext Context => tokens.Context;

        public bool Next()
        {
            do
            {
                if (!tokens.Next())
                    return false;
            } while (tokens.Current is ITriviaToken);

            return true;
        }

        public IEssentialToken Current => (IEssentialToken)tokens.Current;
    }
}
