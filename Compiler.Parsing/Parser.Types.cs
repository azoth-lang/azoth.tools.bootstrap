using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.CST.DeclaredReferenceCapability;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing
{
    public partial class Parser
    {
        public ITypeSyntax ParseType(bool? inferLent)
        {
            var capability = ParseReferenceCapability(inferLent);
            var typeSyntax = ParseTypeWithCapability(capability);

            IQuestionToken? question;
            while ((question = Tokens.AcceptToken<IQuestionToken>()) != null)
            {
                var span = TextSpan.Covering(typeSyntax.Span, question.Span);
                return new OptionalTypeSyntax(span, typeSyntax);
            }

            return typeSyntax;
        }

        public IReferenceCapabilitySyntax? ParseReferenceCapability(bool? inferLent)
        {
            var tokens = new List<ICapabilityToken>();
            ILentKeywordToken? lentKeyword;
            if (Tokens.Current is ILentKeywordToken)
            {
                lentKeyword = Tokens.RequiredToken<ILentKeywordToken>();
                tokens.Add(lentKeyword);
            }
            else
                lentKeyword = null;

            switch (Tokens.Current)
            {
                case IIsolatedKeywordToken _:
                {
                    var isoKeyword = Tokens.RequiredToken<IIsolatedKeywordToken>();
                    tokens.Add(isoKeyword);
                    var span = TextSpan.Covering(lentKeyword?.Span, isoKeyword.Span);
                    var capability = lentKeyword is null ? Isolated : LentIsolated;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
                case ITransitionKeywordToken _:
                {
                    var trnKeyword = Tokens.RequiredToken<ITransitionKeywordToken>();
                    //TODO tokens.Add(trnKeyword);
                    var span = TextSpan.Covering(lentKeyword?.Span, trnKeyword.Span);
                    var capability = lentKeyword is null ? Transition : LentTransition;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
                case IConstKeywordToken _:
                {
                    var constKeyword = Tokens.RequiredToken<IConstKeywordToken>();
                    tokens.Add(constKeyword);
                    var span = TextSpan.Covering(lentKeyword?.Span, constKeyword.Span);
                    var capability = lentKeyword is null ? Const : LentConst;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
                case IMutableKeywordToken _:
                {
                    var mutableKeyword = Tokens.RequiredToken<IMutableKeywordToken>();
                    tokens.Add(mutableKeyword);
                    var span = TextSpan.Covering(lentKeyword?.Span, mutableKeyword.Span);

                    DeclaredReferenceCapability capability;
                    if (lentKeyword is null)
                        capability = inferLent switch
                        {
                            true => LentMutable,
                            false => SharedMutable,
                            _ => Mutable
                        };
                    else
                        capability = LentMutable;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
                case ISharedKeywordToken sharedKeyword:
                {
                    tokens.Add(sharedKeyword);
                    if (lentKeyword != null) Tokens.UnexpectedToken();
                    else Tokens.RequiredToken<ISharedKeywordToken>();
                    var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
                    if (mutableKeyword != null) tokens.Add(mutableKeyword);
                    var span = TextSpan.Covering(lentKeyword?.Span, sharedKeyword?.Span, mutableKeyword?.Span);
                    DeclaredReferenceCapability capability;
                    if (lentKeyword is null)
                        capability = mutableKeyword is null ? Shared : SharedMutable;
                    else
                        capability = mutableKeyword is null ? Lent : LentMutable;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
                case IIdKeywordToken _:
                {
                    // TODO If lent then error, but just make it an id
                    var idKeyword = Tokens.RequiredToken<IIdKeywordToken>();
                    tokens.Add(idKeyword);
                    var span = TextSpan.Covering(lentKeyword?.Span, idKeyword.Span);
                    return new ReferenceCapabilitySyntax(span, tokens, Identity);
                }
                default:
                {
                    if (lentKeyword is null)
                        //Could be a readable reference capability, or could be a value type
                        return null;

                    var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
                    if (mutableKeyword != null)
                        tokens.Add(mutableKeyword);
                    var span = TextSpan.Covering(lentKeyword.Span, mutableKeyword?.Span);
                    var capability = mutableKeyword is null ? Lent : LentMutable;
                    return new ReferenceCapabilitySyntax(span, tokens, capability);
                }
            }
        }

        public ITypeSyntax ParseTypeWithCapability(IReferenceCapabilitySyntax? capability)
        {
            if (capability is null) return ParseBareType();

            var referent = ParseBareType();
            var span = TextSpan.Covering(capability.Span, referent.Span);
            return new CapabilityTypeSyntax(capability!, referent, span);
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
