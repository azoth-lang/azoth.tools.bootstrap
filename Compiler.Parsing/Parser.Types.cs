using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.CST.DeclaredReferenceCapability;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing
{
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
                    var isoKeyword = Tokens.RequiredToken<IIsolatedKeywordToken>();
                    return new ReferenceCapabilitySyntax(isoKeyword.Span, isoKeyword.Yield(), Isolated);
                }
                case IConstKeywordToken _:
                {
                    var constKeyword = Tokens.RequiredToken<IConstKeywordToken>();
                    return new ReferenceCapabilitySyntax(constKeyword.Span, constKeyword.Yield(), Constant);
                }
                case IMutableKeywordToken _:
                {
                    var mutableKeyword = Tokens.RequiredToken<IMutableKeywordToken>();
                    return new ReferenceCapabilitySyntax(mutableKeyword.Span, mutableKeyword.Yield(), Mutable);
                }
                case ILentKeywordToken lentKeyword:
                {
                    var tokens = new List<ICapabilityToken> { lentKeyword };
                    Tokens.RequiredToken<ILentKeywordToken>();
                    var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
                    if (mutableKeyword != null) tokens.Add(mutableKeyword);
                    var span = TextSpan.Covering(lentKeyword.Span, mutableKeyword?.Span);
                    var capability = mutableKeyword is null ? LentReadable : LentMutable;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
                case ISharedKeywordToken sharedKeyword:
                {
                    var tokens = new List<ICapabilityToken> { sharedKeyword };
                    Tokens.RequiredToken<ISharedKeywordToken>();
                    var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
                    if (mutableKeyword != null) tokens.Add(mutableKeyword);
                    var span = TextSpan.Covering(sharedKeyword.Span, mutableKeyword?.Span);
                    var capability = mutableKeyword is null ? SharedReadable : SharedMutable;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
                case IIdKeywordToken _:
                {
                    var idKeyword = Tokens.RequiredToken<IIdKeywordToken>();
                    return new ReferenceCapabilitySyntax(idKeyword.Span, idKeyword.Yield(), Identity);
                }
                default:
                    //Could be a readable reference capability, or could be a value type
                    return null;
            }
        }

        public ITypeSyntax ParseTypeWithCapability(IReferenceCapabilitySyntax? capability)
        {
            var type = ParseBareType();
            if (capability != null)
            {
                var span = TextSpan.Covering(capability.Span, type.Span);
                type = new CapabilityTypeSyntax(capability!, type, span);
            }

            IQuestionToken? question;
            while ((question = Tokens.AcceptToken<IQuestionToken>()) != null)
            {
                var span = TextSpan.Covering(type.Span, question.Span);
                type = new OptionalTypeSyntax(span, type);
            }

            return type;
        }

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
            return new TypeNameSyntax(identifier.Span, name);
        }

        private ITypeNameSyntax ParsePrimitiveType()
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

            return new TypeNameSyntax(keyword.Span, name);
        }
    }
}
