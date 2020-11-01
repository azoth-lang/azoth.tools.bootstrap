using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing
{
    internal class ModifierParser : RecursiveDescentParser
    {
        public ModifierParser(ITokenIterator<IEssentialToken> tokens)
            : base(tokens) { }

        public IAccessModifierToken? ParseAccessModifier()
        {
            return Tokens.Current switch
            {
                IAccessModifierToken _ => Tokens.RequiredToken<IAccessModifierToken>(),
                _ => null
            };
        }

        public IMutableKeywordToken? ParseMutableModifier()
        {
            return Tokens.Current is IMutableKeywordToken ? Tokens.RequiredToken<IMutableKeywordToken>() : null;
        }

        public void ParseEndOfModifiers()
        {
            while (!(Tokens.Current is IEndOfFileToken))
            {
                Tokens.UnexpectedToken();
            }
        }
    }
}
