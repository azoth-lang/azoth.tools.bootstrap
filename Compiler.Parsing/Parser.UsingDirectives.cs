using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public IFixedList<IUsingDirectiveSyntax> ParseUsingDirectives()
        => AcceptMany(AcceptUsingDirective);

    public IUsingDirectiveSyntax? AcceptUsingDirective()
    {
        var accept = Tokens.AcceptToken<IImportKeywordToken>();
        if (accept is null)
            return null;
        var identifiers = ParseManySeparated<(IIdentifierToken?, TextSpan), IDotToken>(
            () => Tokens.ExpectToken<IIdentifierToken>());
        NamespaceName name = NamespaceName.Global;
        foreach (var (identifier, _) in identifiers)
            if (identifier is not null)
                name = name.Qualify(identifier.Value);
        var semicolon = Tokens.Expect<ISemicolonToken>();
        var span = TextSpan.Covering(accept.Span, semicolon);
        return IUsingDirectiveSyntax.Create(span, name);
    }
}
