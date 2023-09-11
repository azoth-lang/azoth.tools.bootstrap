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

        while (Tokens.AcceptToken<IQuestionToken>() is IQuestionToken question)
        {
            var span = TextSpan.Covering(type.Span, question.Span);
            type = new OptionalTypeSyntax(span, type);
        }

        return type;
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
            _ => ParseSimpleTypeName()
        };
    }

    private ISimpleTypeNameSyntax ParseSimpleTypeName()
    {
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        return new SimpleTypeNameSyntax(identifier.Span, name);
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
            IByteKeywordToken _ => SpecialTypeName.Byte,
            IIntKeywordToken _ => SpecialTypeName.Int,
            IUIntKeywordToken _ => SpecialTypeName.UInt,
            ISizeKeywordToken _ => SpecialTypeName.Size,
            IOffsetKeywordToken _ => SpecialTypeName.Offset,
            _ => throw ExhaustiveMatch.Failed(keyword)
        };

        return new SimpleTypeNameSyntax(keyword.Span, name);
    }
}
