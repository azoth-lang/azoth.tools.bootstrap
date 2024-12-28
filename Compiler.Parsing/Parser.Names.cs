using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private IStandardNameExpressionSyntax ParseStandardName()
    {
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        var optionalGenerics = AcceptGenericTypeArguments();
        if (optionalGenerics is not { } genericArguments)
            return IIdentifierNameExpressionSyntax.Create(identifier.Span, name);

        var span = TextSpan.Covering(identifier.Span, genericArguments.Span);
        return IGenericNameExpressionSyntax.Create(span, name, genericArguments.Arguments);
    }
}
