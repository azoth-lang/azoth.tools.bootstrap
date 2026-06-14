using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    #region Method Clauses
    public IFixedList<IOverridesOrHidesSyntax> ParseOverridesOrHidesClauses()
        => AcceptMany(AcceptOverridesOrHidesClause);

    private IOverridesOrHidesSyntax? AcceptOverridesOrHidesClause()
    {
        var overridesOrHidesToken = Tokens.AcceptToken<IOverridesOrHidesToken>();
        if (overridesOrHidesToken is null) return null;
        var accessModifier = AcceptAccessModifier();
        var identifier = Tokens.AcceptToken<IIdentifierToken>();
        IdentifierName? identifierName = identifier is null ? null : (IdentifierName)identifier.Value;
        var optionalParameterTypes = AcceptParameterTypes();
        var @return = ParseReturn();

        var span = TextSpan.Covering(overridesOrHidesToken.Span, accessModifier?.Span,
            identifier?.Span, optionalParameterTypes?.Span, @return?.Span);
        return IOverridesOrHidesSyntax.Create(span, overridesOrHidesToken, accessModifier,
            identifierName, optionalParameterTypes?.Types, @return);
    }

    private (IFixedList<ITypeSyntax> Types, TextSpan Span)? AcceptParameterTypes()
    {
        var openParen = Tokens.AcceptToken<IOpenParenToken>();
        if (openParen is null) return null;
        var parameters = ParseManySeparated<ITypeSyntax, ICommaToken, ICloseParenToken>(ParseType);
        var closeParenSpan = Tokens.Required<ICloseParenToken>();
        var span = TextSpan.Covering(openParen.Span, closeParenSpan);
        return (parameters.ToFixedList(), span);
    }
    #endregion
}
