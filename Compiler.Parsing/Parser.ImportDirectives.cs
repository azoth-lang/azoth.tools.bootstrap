using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public IFixedList<IImportDirectiveSyntax> ParseImportDirectives()
        => AcceptMany(AcceptImportDirective);

    public IImportDirectiveSyntax? AcceptImportDirective()
    {
        var import = Tokens.AcceptToken<IImportKeywordToken>();
        if (import is null)
            return null;
        var identifiers = ParseManySeparated<IIdentifierToken?, IDotToken>(
            Tokens.ExpectToken<IIdentifierToken>);
        NamespaceName name = NamespaceName.Global;
        foreach (var identifier in identifiers.WhereNotNull())
            name = name.Qualify(identifier.Value);
        var semicolon = Tokens.Expect<ISemicolonToken>();
        var span = TextSpan.Covering(import.Span, semicolon);
        return IImportDirectiveSyntax.Create(span, name);
    }
}
