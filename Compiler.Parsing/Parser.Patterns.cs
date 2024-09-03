using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

public partial class Parser
{
    private IPatternSyntax ParsePattern(bool? isMutableBinding = null, bool refutable = true)
    {
        switch (Tokens.Current)
        {
            case ILetKeywordToken _ when isMutableBinding is null:
                var let = Tokens.Consume<IBindingToken>();
                return ParseRestOfBindingContextPattern(let, false, refutable);
            case IVarKeywordToken _ when isMutableBinding is null:
                var var = Tokens.Consume<IBindingToken>();
                return ParseRestOfBindingContextPattern(var, true, refutable);
            case IIdentifierToken _ when isMutableBinding is not null:
                return ParseBindingPattern(isMutableBinding.Value);
            default:
                Add(ParseError.UnexpectedEndOfPattern(File, Tokens.Current.Span.AtStart()));
                throw new ParseFailedException("Unexpected end of pattern");
        }
    }

    private IBindingContextPatternSyntax ParseRestOfBindingContextPattern(
        TextSpan binding, bool isMutableBinding, bool refutable)
    {
        var pattern = ParsePattern(isMutableBinding, refutable);
        ITypeSyntax? type = null;
        if (refutable && Tokens.Current is IColonToken)
        {
            _ = Tokens.Consume<IColonToken>();
            type = ParseType();
        }
        var span = TextSpan.Covering(binding, pattern.Span, type?.Span);
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
