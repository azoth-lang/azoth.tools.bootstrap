using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
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
            ICapabilityToken or IIdentifierToken or IBuiltInTypeToken or IOpenParenToken
                or ISelfKeywordToken or ISelfTypeKeywordToken or IRefKeywordToken
                or ICapabilitySetToken
                => ParseType(),
            _ => null
        };
    }

    private ITypeSyntax ParseType()
    {
        var type = Tokens.Current switch
        {
            ISelfKeywordToken => ParseSelfViewpointType(),
            IRefKeywordToken => ParseRefType(),
            IExplicitCapabilityToken => ParseTypeWithExplicitCapabilityViewpoint(),
            ICapabilityToken => ParseTypeStartingWithCapability(),
            ICapabilitySetToken => ParseCapabilitySetType(),
            IOpenParenToken => ParseFunctionType(),
            _ => ParseBareType()
        };
        return ParseOptionalType(type);
    }

    private ITypeSyntax ParseTypeStartingWith(ICapabilitySyntax? capability)
    {
        if (capability is null) return ParseType();

        var type = ParseTypeStartingWithCapability(capability);
        return ParseOptionalType(type);
    }

    private ITypeSyntax ParseSelfViewpointType()
    {
        var self = Tokens.Consume<ISelfKeywordToken>();
        _ = Tokens.Required<IRightTriangleToken>();
        var type = ParseBareOrRefType();
        var span = TextSpan.Covering(self, type.Span);
        return ISelfViewpointTypeSyntax.Create(span, type);
    }

    private ITypeSyntax ParseTypeWithExplicitCapabilityViewpoint()
    {
        var capability = ParseExplicitCapability();
        _ = Tokens.Required<IRightTriangleToken>();
        var type = ParseBareType();
        var span = TextSpan.Covering(capability.Span, type.Span);
        return ICapabilityViewpointTypeSyntax.Create(span, capability, type);
    }

    private ITypeSyntax ParseTypeStartingWithCapability()
    {
        var capability = ParseStandardCapability();
        return ParseTypeStartingWithCapability(capability);
    }

    private ITypeSyntax ParseTypeStartingWithCapability(ICapabilitySyntax capability)
    {
        var isViewpointType = Tokens.Accept<IRightTriangleToken>();
        var type = ParseBareType();
        if (isViewpointType)
        {
            var span = TextSpan.Covering(capability.Span, type.Span);
            return ICapabilityViewpointTypeSyntax.Create(span, capability, type);
        }
        else
        {
            var span = TextSpan.Covering(capability.Span, type.Span);
            return ICapabilityTypeSyntax.Create(span, capability, type);
        }
    }

    private ITypeSyntax ParseCapabilitySetType()
    {
        var capabilitySet = ParseCapabilitySet();
        var type = ParseBareType();
        var span = TextSpan.Covering(capabilitySet.Span, type.Span);
        return ICapabilitySetTypeSyntax.Create(span, capabilitySet, type);
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
                type = IOptionalTypeSyntax.Create(span, type);
                return true;
            }
            case IQuestionQuestionToken:
            {
                var questionQuestion = Tokens.ConsumeToken<IQuestionQuestionToken>();
                var span = TextSpan.Covering(type.Span, questionQuestion.FirstQuestionSpan);
                type = IOptionalTypeSyntax.Create(span, type);
                span = TextSpan.Covering(type.Span, questionQuestion.SecondQuestionSpan);
                type = IOptionalTypeSyntax.Create(span, type);
                return true;
            }
            default:
                return false;
        }
    }

    private ITypeSyntax ParseBareOrRefType()
        => Tokens.Current switch
        {
            IRefKeywordToken => ParseRefType(),
            _ => ParseBareType()
        };

    private IRefTypeSyntax ParseRefType()
    {
        var refToken = Tokens.ConsumeToken<IRefKeywordToken>();
        var isInternal = refToken is IInternalRefKeywordToken;
        var isVarBinding = Tokens.Accept<IVarKeywordToken>();
        var referent = ParseType();
        var span = TextSpan.Covering(refToken.Span, referent.Span);
        return IRefTypeSyntax.Create(span, isInternal, isVarBinding, referent);
    }

    /// <summary>
    /// Parse a bare type. That is a type that is expected to follow a reference capability.
    /// </summary>
    private ITypeSyntax ParseBareType()
        => Tokens.Current switch
        {
            IBuiltInTypeToken _ => ParseBuiltInType(),
            ISelfTypeKeywordToken => ParseSelfType(),
            // otherwise we want a type name
            _ => ParseOrdinaryTypeName()
        };

    private IFixedList<INameSyntax> ParseTypeNames()
        => AcceptManySeparated<INameSyntax, ICommaToken>(ParseTypeName);

    private INameSyntax ParseTypeName()
        => Tokens.Current switch
        {
            IBuiltInTypeToken _ => ParseBuiltInType(),
            ISelfTypeKeywordToken => ParseSelfType(),
            // otherwise we want a type name
            _ => ParseOrdinaryTypeName()
        };

    private IOrdinaryNameSyntax ParseOrdinaryTypeName()
    {
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        var optionalGenerics = AcceptGenericArguments();
        if (optionalGenerics is { } generics)
            return IGenericNameSyntax.Create(TextSpan.Covering(identifier.Span, generics.Span),
                name, generics.Arguments);

        return IIdentifierNameSyntax.Create(identifier.Span, name);
    }

    private (IFixedList<ITypeSyntax> Arguments, TextSpan Span)? AcceptGenericArguments()
    {
        var openBracket = Tokens.AcceptToken<IOpenBracketToken>();
        if (openBracket is null) return null;
        var arguments = AcceptManySeparated<ITypeSyntax, ICommaToken>(AcceptType);
        var closeBracketSpan = Tokens.Required<ICloseBracketToken>();
        return (arguments, TextSpan.Covering(openBracket.Span, closeBracketSpan));
    }

    private IBuiltInTypeNameSyntax ParseBuiltInType()
    {
        var keyword = Tokens.ConsumeToken<IBuiltInTypeToken>();
        BuiltInTypeName name = keyword switch
        {
            IVoidKeywordToken _ => BuiltInTypeName.Void,
            INeverKeywordToken _ => BuiltInTypeName.Never,
            IBoolKeywordToken _ => BuiltInTypeName.Bool,
            IAnyTypeKeywordToken _ => BuiltInTypeName.Any,

            IIntKeywordToken _ => BuiltInTypeName.Int,
            IUIntKeywordToken _ => BuiltInTypeName.UInt,

            IInt8KeywordToken _ => BuiltInTypeName.Int8,
            IByteKeywordToken _ => BuiltInTypeName.Byte,
            IInt16KeywordToken _ => BuiltInTypeName.Int16,
            IUInt16KeywordToken _ => BuiltInTypeName.UInt16,
            IInt32KeywordToken _ => BuiltInTypeName.Int32,
            IUInt32KeywordToken _ => BuiltInTypeName.UInt32,
            IInt64KeywordToken _ => BuiltInTypeName.Int64,
            IUInt64KeywordToken _ => BuiltInTypeName.UInt64,

            ISizeKeywordToken _ => BuiltInTypeName.Size,
            IOffsetKeywordToken _ => BuiltInTypeName.Offset,

            INIntKeywordToken _ => BuiltInTypeName.NInt,
            INUIntKeywordToken _ => BuiltInTypeName.NUInt,
            _ => throw ExhaustiveMatch.Failed(keyword)
        };

        return IBuiltInTypeNameSyntax.Create(keyword.Span, name);
    }

    private IBuiltInTypeNameSyntax ParseSelfType()
    {
        var keyword = Tokens.ConsumeToken<ISelfTypeKeywordToken>();
        return IBuiltInTypeNameSyntax.Create(keyword.Span, BuiltInTypeName.Self);
    }

    private IFunctionTypeSyntax ParseFunctionType()
    {
        var openParen = Tokens.ConsumeToken<IOpenParenToken>();
        var parameterTypes = AcceptManySeparated<IParameterTypeSyntax, ICommaToken>(AcceptParameterType);
        Tokens.Required<ICloseParenToken>();
        Tokens.Required<IRightArrowToken>();
        var returnType = ParseType();
        var span = TextSpan.Covering(openParen.Span, returnType.Span);
        return IFunctionTypeSyntax.Create(span, parameterTypes, returnType);
    }

    private IParameterTypeSyntax? AcceptParameterType()
    {
        var lent = Tokens.AcceptToken<ILentKeywordToken>();
        var referent = lent is null ? AcceptType() : ParseType();
        if (referent is null)
            return null;
        var span = TextSpan.Covering(lent?.Span, referent.Span);
        return IParameterTypeSyntax.Create(span, lent is not null, referent);
    }
}
