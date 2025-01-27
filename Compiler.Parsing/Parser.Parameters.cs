using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public INamedParameterSyntax ParseFunctionParameter()
    {
        var lentBinding = Tokens.AcceptToken<ILentKeywordToken>();
        return ParseFunctionParameter(lentBinding);
    }

    public INamedParameterSyntax ParseFunctionParameter(ILentKeywordToken? lentBinding)
    {
        bool isLentBinding = lentBinding is not null;
        var mutableBinding = Tokens.AcceptToken<IVarKeywordToken>();
        bool isMutableBinding = mutableBinding is not null;
        if (isLentBinding && isMutableBinding)
            Add(ParseError.LentVarNotAllowed(File, TextSpan.Covering(lentBinding!.Span, mutableBinding!.Span)));
        var identifier = Tokens.RequiredToken<IIdentifierOrUnderscoreToken>();
        var name = identifier.Value;
        var nameSpan = identifier.Span;
        Tokens.Expect<IColonToken>();
        var type = ParseType();
        IExpressionSyntax? defaultValue = null;
        if (Tokens.Accept<IEqualsToken>()) defaultValue = ParseExpression();
        var span = TextSpan.Covering(lentBinding?.Span, mutableBinding?.Span, type.Span, defaultValue?.Span);
        return INamedParameterSyntax.Create(span, nameSpan, isMutableBinding, isLentBinding, name, type, defaultValue);
    }

    public IParameterSyntax ParseMethodParameter()
    {
        var lentBinding = Tokens.AcceptToken<ILentKeywordToken>();
        return Tokens.Current switch
        {
            IStandardCapabilityToken or ICapabilitySetToken or ISelfKeywordToken
                => ParseMethodSelfParameter(lentBinding),
            _ => ParseFunctionParameter(lentBinding),
        };
    }

    public IParameterSyntax ParseInitializerParameter()
    {
        var lentBinding = Tokens.AcceptToken<ILentKeywordToken>();
        switch (Tokens.Current)
        {

            case IStandardCapabilityToken:
            case ISelfKeywordToken:
                return ParseInitializerSelfParameter(lentBinding);
            case IDotToken _:
            {
                if (lentBinding is not null) Add(ParseError.LentFieldParameter(File, lentBinding.Span));

                var dot = Tokens.Consume<IDotToken>();
                var identifier = Tokens.RequiredToken<IIdentifierToken>();
                var equals = Tokens.AcceptToken<IEqualsToken>();
                IExpressionSyntax? defaultValue = null;
                if (equals is not null) defaultValue = ParseExpression();
                var span = TextSpan.Covering(dot, identifier.Span, defaultValue?.Span);
                IdentifierName name = identifier.Value;
                return IFieldParameterSyntax.Create(span, name, defaultValue);
            }
            default:
                return ParseFunctionParameter(lentBinding);
        }
    }

    private IInitializerSelfParameterSyntax ParseInitializerSelfParameter(ILentKeywordToken? lentBinding)
    {
        bool isLentBinding = lentBinding is not null;
        var referenceCapability = ParseStandardCapability();
        var selfSpan = Tokens.Expect<ISelfKeywordToken>();
        var span = TextSpan.Covering(lentBinding?.Span, referenceCapability.Span, selfSpan);
        return IInitializerSelfParameterSyntax.Create(span, isLentBinding, referenceCapability);
    }

    private IMethodSelfParameterSyntax ParseMethodSelfParameter(ILentKeywordToken? lentBinding)
    {
        bool isLentBinding = lentBinding is not null;
        var referenceCapability = ParseStandardCapabilityConstraint();
        var selfSpan = Tokens.Expect<ISelfKeywordToken>();
        var span = TextSpan.Covering(lentBinding?.Span, referenceCapability.Span, selfSpan);
        return IMethodSelfParameterSyntax.Create(span, isLentBinding, referenceCapability);
    }
}
