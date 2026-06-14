using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser : SharedModifierParser
{
    public Parser(ITokenIterator<IEssentialToken> tokens)
        : base(tokens)
    { }
}
