using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public INamedParameterSyntax ParseFunctionParameter()
    {
        var span = Tokens.Current.Span;
        var mutableBinding = Tokens.Accept<IVarKeywordToken>();
        var identifier = Tokens.RequiredToken<IIdentifierOrUnderscoreToken>();
        var name = identifier.Value;
        Tokens.Expect<IColonToken>();
        var type = ParseType();
        IExpressionSyntax? defaultValue = null;
        if (Tokens.Accept<IEqualsToken>()) defaultValue = ParseExpression();
        span = TextSpan.Covering(span, type.Span, defaultValue?.Span);
        return new NamedParameterSyntax(span, mutableBinding, name, type, defaultValue);
    }

    public IParameterSyntax ParseMethodParameter()
    {
        switch (Tokens.Current)
        {
            case ICapabilityToken:
            case ISelfKeywordToken:
                return ParseSelfParameter();
            default:
                return ParseFunctionParameter();
        }
    }

    public IParameterSyntax ParseConstructorParameter()
    {
        switch (Tokens.Current)
        {
            case ICapabilityToken:
            case ISelfKeywordToken:
                return ParseSelfParameter();
            case IDotToken _:
            {
                var dot = Tokens.Expect<IDotToken>();
                var identifier = Tokens.RequiredToken<IIdentifierToken>();
                var equals = Tokens.AcceptToken<IEqualsToken>();
                IExpressionSyntax? defaultValue = null;
                if (equals is not null) defaultValue = ParseExpression();
                var span = TextSpan.Covering(dot, identifier.Span, defaultValue?.Span);
                Name name = identifier.Value;
                return new FieldParameterSyntax(span, name, defaultValue);
            }
            default:
                return ParseFunctionParameter();
        }
    }

    private ISelfParameterSyntax ParseSelfParameter()
    {
        var span = Tokens.Current.Span;
        var referenceCapability = ParseReferenceCapability()
                                  ?? ReferenceCapabilitySyntax.ImplicitReadOnly(Tokens.Current.Span.AtStart());
        var selfSpan = Tokens.Expect<ISelfKeywordToken>();
        span = TextSpan.Covering(span, selfSpan);
        return new SelfParameterSyntax(span, referenceCapability);
    }
}
