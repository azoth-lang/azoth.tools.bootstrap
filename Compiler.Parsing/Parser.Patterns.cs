using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    /// <summary>
    /// Parse a pattern.
    /// </summary>
    /// <param name="inMutableBindingContext">Whether the pattern is inside a binding context and
    /// whether that context is mutable. <see langword="null"/> indicates it is not in a binding
    /// context.</param>
    /// <param name="refutable">Whether the pattern is allowed to be refutable.</param>
    // TODO do not enforce refutableness in the parser. Enforce it in the semantics.
    private IPatternSyntax ParsePattern(bool? inMutableBindingContext = null, bool refutable = true)
    {
        switch (Tokens.Current)
        {
            case IBindingToken when inMutableBindingContext is null:
                return ParseBindingContextPattern(refutable);
            case IIdentifierToken when inMutableBindingContext is not null:
                return ParseBindingPattern(inMutableBindingContext.Value);
            case var _ when inMutableBindingContext is null:
                return ParseTypePattern();
            case var token:
                Add(ParseError.UnexpectedEndOfPattern(File, token.Span.AtStart()));
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
        var identifier = Tokens.ConsumeToken<IIdentifierToken>();
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

    private ITypePatternSyntax ParseTypePattern()
    {
        var type = ParseType();
        return ITypePatternSyntax.Create(type.Span, type);
    }
}
