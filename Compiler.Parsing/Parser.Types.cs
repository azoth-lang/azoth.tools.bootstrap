using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private ITypeSyntax? AcceptType()
    {
        return Tokens.Current switch
        {
            ICapabilityToken or IIdentifierToken or IPrimitiveTypeToken or IOpenParenToken
                or ISelfKeywordToken => ParseType(),
            _ => null
        };
    }

    private ITypeSyntax ParseType()
    {
        switch (Tokens.Current)
        {
            case ISelfKeywordToken:
                return ParseSelfViewpointType();
            case IExplicitCapabilityToken:
                return ParseTypeWithExplicitCapabilityViewpoint();
            default:
                var capability = AcceptStandardCapability();
                if (capability is not null && Tokens.Current is IRightTriangleToken)
                    return ParseTypeWithCapabilityViewpoint(capability);

                return ParseTypeWithCapability(capability);
        }
    }

    private ITypeSyntax ParseSelfViewpointType()
    {
        var self = Tokens.Consume<ISelfKeywordToken>();
        _ = Tokens.Required<IRightTriangleToken>();
        var type = ParseBareType();
        var span = TextSpan.Covering(self, type.Span);
        type = new SelfViewpointTypeSyntax(span, type);
        return ParseOptionalType(type);
    }

    private ITypeSyntax ParseTypeWithCapability(ICapabilitySyntax? capability)
    {
        var type = ParseBareType();
        if (capability is not null)
            type = new CapabilityTypeSyntax(capability, type);

        return ParseOptionalType(type);
    }

    private ITypeSyntax ParseTypeWithCapabilityViewpoint(ICapabilitySyntax capability)
    {
        _ = Tokens.Consume<IRightTriangleToken>();
        var type = ParseBareType();
        type = new CapabilityViewpointTypeSyntax(capability, type);

        return ParseOptionalType(type);
    }

    private ITypeSyntax ParseTypeWithExplicitCapabilityViewpoint()
    {
        var capability = ParseExplicitCapability();
        _ = Tokens.Required<IRightTriangleToken>();
        var type = ParseBareType();
        type = new CapabilityViewpointTypeSyntax(capability, type);

        return ParseOptionalType(type);
    }

    private ITypeSyntax ParseOptionalType(ITypeSyntax type)
    {
        while (TryParseOptionalType(ref type))
        {
            // Work is done by TryParseOptionalType
        }

        return type;
    }

    private bool TryParseOptionalType(ref ITypeSyntax type)
    {
        switch (Tokens.Current)
        {
            case IQuestionToken:
            {
                var question = Tokens.Consume<IQuestionToken>();
                var span = TextSpan.Covering(type.Span, question);
                type = new OptionalTypeSyntax(span, type);
                return true;
            }
            case IQuestionQuestionToken:
            {
                var questionQuestion = Tokens.ConsumeToken<IQuestionQuestionToken>();
                var span = TextSpan.Covering(type.Span, questionQuestion.FirstQuestionSpan);
                type = new OptionalTypeSyntax(span, type);
                span = TextSpan.Covering(type.Span, questionQuestion.SecondQuestionSpan);
                type = new OptionalTypeSyntax(span, type);
                return true;
            }
            default:
                return false;
        }
    }

    /// <summary>
    /// Parse a bare type. That is a type that is expected to follow a reference capability.
    /// </summary>
    private ITypeSyntax ParseBareType()
    {
        return Tokens.Current switch
        {
            IPrimitiveTypeToken _ => ParsePrimitiveType(),
            IOpenParenToken _ => ParseFunctionType(),
            // otherwise we want a type name
            _ => ParseTypeName()
        };
    }

    private ITypeNameSyntax ParseTypeName()
    {
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        var optionalGenerics = AcceptGenericTypeArguments();
        if (optionalGenerics is { } generics)
            return new GenericTypeNameSyntax(TextSpan.Covering(identifier.Span, generics.Span),
                name, generics.Arguments);

        return new IdentifierTypeNameSyntax(identifier.Span, name);
    }

    private IFixedList<ITypeNameSyntax> ParseTypeNames()
        => AcceptManySeparated<ITypeNameSyntax, ICommaToken>(ParseTypeName);

    private (IFixedList<ITypeSyntax> Arguments, TextSpan Span)? AcceptGenericTypeArguments()
    {
        var openBracket = Tokens.AcceptToken<IOpenBracketToken>();
        if (openBracket is null) return null;
        var arguments = AcceptManySeparated<ITypeSyntax, ICommaToken>(AcceptType);
        var closeBracketSpan = Tokens.Required<ICloseBracketToken>();
        return (arguments, TextSpan.Covering(openBracket.Span, closeBracketSpan));
    }

    private ISimpleTypeNameSyntax ParsePrimitiveType()
    {
        var keyword = Tokens.ConsumeToken<IPrimitiveTypeToken>();
        SpecialTypeName name = keyword switch
        {
            IVoidKeywordToken _ => SpecialTypeName.Void,
            INeverKeywordToken _ => SpecialTypeName.Never,
            IBoolKeywordToken _ => SpecialTypeName.Bool,
            IAnyTypeKeywordToken _ => SpecialTypeName.Any,

            IIntKeywordToken _ => SpecialTypeName.Int,
            IUIntKeywordToken _ => SpecialTypeName.UInt,

            IInt8KeywordToken _ => SpecialTypeName.Int8,
            IByteKeywordToken _ => SpecialTypeName.Byte,
            IInt16KeywordToken _ => SpecialTypeName.Int16,
            IUInt16KeywordToken _ => SpecialTypeName.UInt16,
            IInt32KeywordToken _ => SpecialTypeName.Int32,
            IUInt32KeywordToken _ => SpecialTypeName.UInt32,
            IInt64KeywordToken _ => SpecialTypeName.Int64,
            IUInt64KeywordToken _ => SpecialTypeName.UInt64,

            ISizeKeywordToken _ => SpecialTypeName.Size,
            IOffsetKeywordToken _ => SpecialTypeName.Offset,

            INIntKeywordToken _ => SpecialTypeName.NInt,
            INUIntKeywordToken _ => SpecialTypeName.NUInt,
            _ => throw ExhaustiveMatch.Failed(keyword)
        };

        return new SpecialTypeNameSyntax(keyword.Span, name);
    }

    private IFunctionTypeSyntax ParseFunctionType()
    {
        var openParen = Tokens.ConsumeToken<IOpenParenToken>();
        var parameterTypes = AcceptManySeparated<IParameterTypeSyntax, ICommaToken>(AcceptParameterType);
        Tokens.Required<ICloseParenToken>();
        Tokens.Required<IRightArrowToken>();
        var returnType = ParseReturnType();
        var span = TextSpan.Covering(openParen.Span, returnType.Span);
        return new FunctionTypeSyntax(span, parameterTypes, returnType);
    }

    private IParameterTypeSyntax? AcceptParameterType()
    {
        var lent = Tokens.AcceptToken<ILentKeywordToken>();
        var referent = lent is null ? AcceptType() : ParseType();
        if (referent is null)
            return null;
        var span = TextSpan.Covering(lent?.Span, referent.Span);
        return new ParameterTypeSyntax(span, lent is not null, referent);
    }

    private IReturnTypeSyntax ParseReturnType()
    {
        var referent = ParseType();
        return new ReturnTypeSyntax(referent.Span, referent);
    }
}
