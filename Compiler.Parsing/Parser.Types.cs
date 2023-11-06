using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.CST.DeclaredReferenceCapability;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    public ITypeSyntax? AcceptType()
    {
        return Tokens.Current switch
        {
            ICapabilityToken or IIdentifierToken or IPrimitiveTypeToken => ParseType(),
            _ => null
        };
    }

    public ITypeSyntax ParseType()
    {
        var capability = ParseReferenceCapability();
        return ParseTypeWithCapability(capability);
    }

    public IReferenceCapabilitySyntax? ParseReferenceCapability()
    {
        switch (Tokens.Current)
        {
            case IIsolatedKeywordToken _:
            {
                var isolatedKeyword = Tokens.RequiredToken<IIsolatedKeywordToken>();
                return new ReferenceCapabilitySyntax(isolatedKeyword.Span, isolatedKeyword.Yield(), Isolated);
            }
            case IExclusivelyMutableKeywordToken _:
            {
                var exclusivelyMutableKeyword = Tokens.RequiredToken<IExclusivelyMutableKeywordToken>();
                return new ReferenceCapabilitySyntax(exclusivelyMutableKeyword.Span, exclusivelyMutableKeyword.Yield(), ExclusivelyMutable);
            }
            case IMutableKeywordToken _:
            {
                var mutableKeyword = Tokens.RequiredToken<IMutableKeywordToken>();
                return new ReferenceCapabilitySyntax(mutableKeyword.Span, mutableKeyword.Yield(), Mutable);
            }
            case IConstKeywordToken _:
            {
                var constKeyword = Tokens.RequiredToken<IConstKeywordToken>();
                return new ReferenceCapabilitySyntax(constKeyword.Span, constKeyword.Yield(), Constant);
            }
            case IIdKeywordToken _:
            {
                var idKeyword = Tokens.RequiredToken<IIdKeywordToken>();
                return new ReferenceCapabilitySyntax(idKeyword.Span, idKeyword.Yield(), Identity);
            }
            default:
                // Could be a readable reference capability, or could be a value type
                return null;
        }
    }

    public ITypeSyntax ParseTypeWithCapability(IReferenceCapabilitySyntax? capability)
    {
        var type = ParseBareType();
        if (capability is not null)
        {
            var span = TextSpan.Covering(capability.Span, type.Span);
            type = new CapabilityTypeSyntax(capability, type, span);
        }

        while (TryParseOptionalType(ref type))
        {
            // Work is done by TryParseOptionalType
        }

        return type;
    }

    public bool TryParseOptionalType(ref ITypeSyntax type)
    {
        switch (Tokens.Current)
        {
            case IQuestionToken:
            {
                var question = Tokens.Required<IQuestionToken>();
                var span = TextSpan.Covering(type.Span, question);
                type = new OptionalTypeSyntax(span, type);
                return true;
            }
            case IQuestionQuestionToken:
            {
                var questionQuestion = Tokens.RequiredToken<IQuestionQuestionToken>();
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
            return new ParameterizedTypeSyntax(TextSpan.Covering(identifier.Span, generics.Span),
                name, generics.Arguments);

        return new SimpleTypeNameSyntax(identifier.Span, name);
    }

    private FixedList<ITypeNameSyntax> ParseTypeNames()
        => AcceptManySeparated<ITypeNameSyntax, ICommaToken>(ParseTypeName);

    private (FixedList<ITypeSyntax> Arguments, TextSpan Span)? AcceptGenericTypeArguments()
    {
        var openBracket = Tokens.AcceptToken<IOpenBracketToken>();
        if (openBracket is null) return null;
        var arguments = AcceptManySeparated<ITypeSyntax, ICommaToken>(AcceptType);
        var closeBracketSpan = Tokens.Required<ICloseBracketToken>();
        return (arguments, TextSpan.Covering(openBracket.Span, closeBracketSpan));
    }

    private ISimpleTypeNameSyntax ParsePrimitiveType()
    {
        var keyword = Tokens.RequiredToken<IPrimitiveTypeToken>();
        SpecialTypeName name = keyword switch
        {
            IVoidKeywordToken _ => SpecialTypeName.Void,
            INeverKeywordToken _ => SpecialTypeName.Never,
            IBoolKeywordToken _ => SpecialTypeName.Bool,
            IAnyKeywordToken _ => SpecialTypeName.Any,

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
            _ => throw ExhaustiveMatch.Failed(keyword)
        };

        return new SimpleTypeNameSyntax(keyword.Span, name);
    }
}
