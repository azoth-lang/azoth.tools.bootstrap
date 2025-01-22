using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private IOrdinaryNameSyntax ParseOrdinaryName()
    {
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        if (AcceptGenericArguments() is not { } genericArguments)
            return IIdentifierNameSyntax.Create(identifier.Span, name);

        var span = TextSpan.Covering(identifier.Span, genericArguments.Span);
        return IGenericNameSyntax.Create(span, name, genericArguments.Arguments);
    }
}
