using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private IPatternSyntax ParsePattern(bool? isMutableBinding = null, bool refutable = true)
    {
        switch (Tokens.Current)
        {
            case IBindingToken _ when isMutableBinding is null:
                return ParseBindingContextPattern(refutable);
            case IIdentifierToken _ when isMutableBinding is not null:
                return ParseBindingPattern(isMutableBinding.Value);
            default:
                Add(ParseError.UnexpectedEndOfPattern(File, Tokens.Current.Span.AtStart()));
                throw new ParseFailedException("Unexpected end of pattern");
        }
    }

    private IBindingContextPatternSyntax ParseBindingContextPattern(bool refutable)
    {
        var binding = Tokens.ConsumeToken<IBindingToken>();
        var isMutableBinding = binding switch
        {
            ILetKeywordToken _ => false,
            IVarKeywordToken _ => false,
            _ => throw ExhaustiveMatch.Failed(binding),
        };
        var pattern = ParsePattern(isMutableBinding, refutable);
        ITypeSyntax? type = null;
        if (refutable && Tokens.Current is IColonToken)
        {
            _ = Tokens.Consume<IColonToken>();
            type = ParseType();
        }
        var span = TextSpan.Covering(binding.Span, pattern.Span, type?.Span);
        return IBindingContextPatternSyntax.Create(span, isMutableBinding, pattern, type);
    }

    private IOptionalOrBindingPatternSyntax ParseBindingPattern(bool isMutableBinding)
    {
        var identifier = Tokens.RequiredToken<IIdentifierToken>();
        var name = identifier.Value;
        IOptionalOrBindingPatternSyntax pattern = IBindingPatternSyntax.Create(isMutableBinding, identifier.Span, name);
        while (TryParseOptionalPattern(ref pattern))
        {
            // Work is done by TryParseOptionalPattern
        }
        return pattern;
    }

    private bool TryParseOptionalPattern(ref IOptionalOrBindingPatternSyntax pattern)
    {
        switch (Tokens.Current)
        {
            case IQuestionToken:
            {
                var question = Tokens.Consume<IQuestionToken>();
                var span = TextSpan.Covering(pattern.Span, question);
                pattern = IOptionalPatternSyntax.Create(span, pattern);
                return true;
            }
            case IQuestionQuestionToken:
            {
                var questionQuestion = Tokens.ConsumeToken<IQuestionQuestionToken>();
                var span = TextSpan.Covering(pattern.Span, questionQuestion.FirstQuestionSpan);
                pattern = IOptionalPatternSyntax.Create(span, pattern);
                span = TextSpan.Covering(pattern.Span, questionQuestion.SecondQuestionSpan);
                pattern = IOptionalPatternSyntax.Create(span, pattern);
                return true;
            }
            default:
                return false;
        }
    }
}
