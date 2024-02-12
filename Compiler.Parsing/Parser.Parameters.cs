using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

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
        Tokens.Expect<IColonToken>();
        var type = ParseType();
        IExpressionSyntax? defaultValue = null;
        if (Tokens.Accept<IEqualsToken>()) defaultValue = ParseExpression();
        var span = TextSpan.Covering(lentBinding?.Span, mutableBinding?.Span, type.Span, defaultValue?.Span);
        return new NamedParameterSyntax(span, isMutableBinding, isLentBinding, name, type, defaultValue);
    }

    public IParameterSyntax ParseMethodParameter()
    {
        var lentBinding = Tokens.AcceptToken<ILentKeywordToken>();
        return Tokens.Current switch
        {
            IStandardCapabilityToken or ICapabilityConstraintToken or ISelfKeywordToken
                => ParseMethodSelfParameter(lentBinding),
            _ => ParseFunctionParameter(lentBinding),
        };
    }

    public IParameterSyntax ParseConstructorParameter()
    {
        var lentBinding = Tokens.AcceptToken<ILentKeywordToken>();
        switch (Tokens.Current)
        {

            case IStandardCapabilityToken:
            case ISelfKeywordToken:
                return ParseConstructorSelfParameter(lentBinding);
            case IDotToken _:
            {
                if (lentBinding is not null)
                    Add(ParseError.LentFieldParameter(File, lentBinding.Span));

                var dot = Tokens.Consume<IDotToken>();
                var identifier = Tokens.RequiredToken<IIdentifierToken>();
                var equals = Tokens.AcceptToken<IEqualsToken>();
                IExpressionSyntax? defaultValue = null;
                if (equals is not null) defaultValue = ParseExpression();
                var span = TextSpan.Covering(dot, identifier.Span, defaultValue?.Span);
                SimpleName name = identifier.Value;
                return new FieldParameterSyntax(span, name, defaultValue);
            }
            default:
                return ParseFunctionParameter(lentBinding);
        }
    }

    private IConstructorSelfParameterSyntax ParseConstructorSelfParameter(ILentKeywordToken? lentBinding)
    {
        bool isLentBinding = lentBinding is not null;
        var referenceCapability = AcceptStandardReferenceCapability()
                                  ?? ReferenceCapabilitySyntax.ImplicitReadOnly(Tokens.Current.Span.AtStart());
        var selfSpan = Tokens.Expect<ISelfKeywordToken>();
        var span = TextSpan.Covering(lentBinding?.Span, referenceCapability.Span, selfSpan);
        return new ConstructorSelfParameterSyntax(span, isLentBinding, referenceCapability);
    }

    private IMethodSelfParameterSyntax ParseMethodSelfParameter(ILentKeywordToken? lentBinding)
    {
        bool isLentBinding = lentBinding is not null;
        var referenceCapability = (ISelfReferenceCapabilitySyntax?)AcceptReferenceCapabilityConstraint()
                                  ?? AcceptStandardReferenceCapability()
                                  ?? ReferenceCapabilitySyntax.ImplicitReadOnly(Tokens.Current.Span.AtStart());
        var selfSpan = Tokens.Expect<ISelfKeywordToken>();
        var span = TextSpan.Covering(lentBinding?.Span, referenceCapability.Span, selfSpan);
        return new MethodSelfParameterSyntax(span, isLentBinding, referenceCapability);
    }

    private IReferenceCapabilityConstraintSyntax? AcceptReferenceCapabilityConstraint()
    {
        var constraint = Tokens.AcceptToken<ICapabilityConstraintToken>();
        if (constraint is null) return null;
        return constraint switch
        {
            IReadableKeywordToken _ => new ReferenceCapabilityConstraintSyntax(constraint.Span, ReferenceCapabilityConstraint.Readable),
            _ => throw ExhaustiveMatch.Failed(constraint),
        };
    }
}
