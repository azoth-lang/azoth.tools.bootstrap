using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.Types.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing
{
    public partial class Parser
    {
        public ITypeSyntax ParseType(bool inParameter)
        {
            var typeSyntax = ParseTypeWithCapability(inParameter);

            IQuestionToken? question;
            while ((question = Tokens.AcceptToken<IQuestionToken>()) != null)
            {
                var span = TextSpan.Covering(typeSyntax.Span, question.Span);
                return new OptionalTypeSyntax(span, typeSyntax);
            }

            return typeSyntax;
        }

        private ITypeSyntax ParseTypeWithCapability(bool inParameter)
        {
            var lent = Tokens.Current is ILentKeywordToken
                ? Tokens.RequiredToken<ILentKeywordToken>() : null;

            switch (Tokens.Current)
            {
                case IIsolatedKeywordToken _:
                {
                    var isoKeyword = Tokens.RequiredToken<IIsolatedKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(lent?.Span, isoKeyword.Span, referent.Span);
                    var capability = lent is null ? Isolated : LentIsolated;
                    return new CapabilityTypeSyntax(capability, referent, span);
                }
                case ITransitionKeywordToken _:
                {
                    var trnKeyword = Tokens.RequiredToken<ITransitionKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(lent?.Span, trnKeyword.Span, referent.Span);
                    var capability = lent is null ? Transition : LentTransition;
                    return new CapabilityTypeSyntax(capability, referent, span);
                }
                case IConstKeywordToken _:
                {
                    var constKeyword = Tokens.RequiredToken<IConstKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(lent?.Span, constKeyword.Span, referent.Span);
                    var capability = lent is null ? Const : LentConst;
                    return new CapabilityTypeSyntax(capability, referent, span);
                }
                case IMutableKeywordToken _:
                {
                    var mutableKeyword = Tokens.RequiredToken<IMutableKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(lent?.Span, mutableKeyword.Span, referent.Span);
                    var capability = lent != null || inParameter ? LentMutable : SharedMutable;
                    return new CapabilityTypeSyntax(capability, referent, span);
                }
                case ISharedKeywordToken _:
                {
                    if (lent != null) Tokens.UnexpectedToken();
                    // TODO If lent then error on the shared afterward. Make it lent
                    // We may have consumed the shared keyword token if it came after `lent`
                    var sharedKeyword = Tokens.AcceptToken<ISharedKeywordToken>();
                    var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(lent?.Span, sharedKeyword?.Span, mutableKeyword?.Span, referent.Span);
                    ReferenceCapability capability;
                    if (lent is null)
                        capability = mutableKeyword is null ? Shared : SharedMutable;
                    else
                        capability = mutableKeyword is null ? Lent : LentMutable;
                    return new CapabilityTypeSyntax(capability, referent, span);
                }
                case IIdKeywordToken _:
                {
                    // TODO If lent then error, but just make it an id
                    var idKeyword = Tokens.RequiredToken<IIdKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(lent?.Span, idKeyword.Span, referent.Span);
                    return new CapabilityTypeSyntax(Identity, referent, span);
                }
                default:
                {
                    if (lent is null)
                        // Could be a value type
                        return ParseBareType();

                    var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(lent.Span, mutableKeyword?.Span, referent.Span);
                    var capability = mutableKeyword is null ? Lent : LentMutable;
                    return new CapabilityTypeSyntax(capability, referent, span);
                }
            }
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
