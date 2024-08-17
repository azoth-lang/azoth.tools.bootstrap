using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.Syntax.DeclaredCapability;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private ICapabilitySyntax? AcceptStandardCapability()
        => Tokens.Current switch
        {
            ICapabilityToken _ => ParseStandardCapability(),
            _ => null
        };

    private ICapabilitySyntax ParseStandardCapability()
    {
        switch (Tokens.Current)
        {
            case IIsolatedKeywordToken _:
            {
                var isolatedKeyword = Tokens.ConsumeToken<IIsolatedKeywordToken>();
                return ICapabilitySyntax.Create(isolatedKeyword.Span, isolatedKeyword.Yield(), Isolated);
            }
            case IMutableKeywordToken _:
            {
                var mutableKeyword = Tokens.ConsumeToken<IMutableKeywordToken>();
                return ICapabilitySyntax.Create(mutableKeyword.Span, mutableKeyword.Yield(), Mutable);
            }
            case IConstKeywordToken _:
            {
                var constKeyword = Tokens.ConsumeToken<IConstKeywordToken>();
                return ICapabilitySyntax.Create(constKeyword.Span, constKeyword.Yield(), Constant);
            }
            case IIdKeywordToken _:
            {
                var idKeyword = Tokens.ConsumeToken<IIdKeywordToken>();
                return ICapabilitySyntax.Create(idKeyword.Span, idKeyword.Yield(), Identity);
            }
            case ITempKeywordToken _:
            {
                var tempKeyword = Tokens.ConsumeToken<ITempKeywordToken>();
                DeclaredCapability declaredCapability;
                switch (Tokens.Current)
                {
                    case IIsolatedKeywordToken _:
                        declaredCapability = TemporarilyIsolated;
                        break;
                    case IConstKeywordToken _:
                        declaredCapability = TemporarilyConstant;
                        break;
                    case IStandardCapabilityToken capabilityToken:
                    {
                        var errorSpan = TextSpan.Covering(tempKeyword.Span, capabilityToken.Span);
                        Add(ParseError.InvalidTempCapability(File, errorSpan));
                        return ParseStandardCapability();
                    }
                    default:
                        // Treat this as a read-only reference capability
                        Add(ParseError.InvalidTempCapability(File, tempKeyword.Span));
                        return ICapabilitySyntax.Create(tempKeyword.Span, Enumerable.Empty<IStandardCapabilityToken>(), Read);
                }

                var capabilityKeyword = Tokens.ConsumeToken<IStandardCapabilityToken>();
                var span = TextSpan.Covering(tempKeyword.Span, capabilityKeyword.Span);
                var tokens = tempKeyword.Yield<IStandardCapabilityToken>().Append(capabilityKeyword);
                return ICapabilitySyntax.Create(span, tokens, declaredCapability);
            }
            case IReadKeywordToken _:
            {
                var readKeyword = Tokens.ConsumeToken<IReadKeywordToken>();
                Add(ParseError.ExplicitRead(File, readKeyword.Span));
                return ICapabilitySyntax.Create(readKeyword.Span, readKeyword.Yield(), Read);
            }
            default:
                return ICapabilitySyntax.CreateImplicitReadOnly(Tokens.Current.Span.AtStart());
        }
    }

    private ICapabilitySyntax ParseExplicitCapability()
    {
        var capability = Tokens.RequiredToken<IExplicitCapabilityToken>();
        return capability switch
        {
            IReadKeywordToken readKeyword => ICapabilitySyntax.Create(capability.Span, readKeyword.Yield(), Read),
            _ => throw ExhaustiveMatch.Failed(capability),
        };
    }

    private ICapabilitySetSyntax ParseCapabilitySet()
    {
        var constraint = Tokens.RequiredToken<ICapabilitySetToken>();
        return constraint switch
        {
            IReadableKeywordToken _ => ICapabilitySetSyntax.Create(constraint.Span, CapabilitySet.Readable),
            IShareableKeywordToken _ => ICapabilitySetSyntax.Create(constraint.Span, CapabilitySet.Shareable),
            IAliasableKeywordToken _ => ICapabilitySetSyntax.Create(constraint.Span, CapabilitySet.Aliasable),
            ISendableKeywordToken _ => ICapabilitySetSyntax.Create(constraint.Span, CapabilitySet.Sendable),
            IAnyKeywordToken _ => ICapabilitySetSyntax.Create(constraint.Span, CapabilitySet.Any),
            _ => throw ExhaustiveMatch.Failed(constraint),
        };
    }

    private ICapabilityConstraintSyntax ParseStandardCapabilityConstraint()
        => AcceptStandardCapabilityConstraint() ?? ICapabilitySyntax.CreateImplicitReadOnly(Tokens.Current.Span.AtStart());

    private ICapabilityConstraintSyntax? AcceptStandardCapabilityConstraint()
        => Tokens.Current switch
        {
            ICapabilityToken _ => ParseStandardCapability(),
            ICapabilitySetToken _ => ParseCapabilitySet(),
            _ => null,
        };

    private ICapabilityConstraintSyntax? AcceptExplicitCapabilityConstraint()
        => Tokens.Current switch
        {
            IStandardCapabilityToken _ => ParseStandardCapability(),
            IExplicitCapabilityToken _ => ParseExplicitCapability(),
            ICapabilitySetToken _ => ParseCapabilitySet(),
            _ => null,
        };
}
