using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

public partial interface IQuestionQuestionToken
{
    public TextSpan FirstQuestionSpan => new(Span.Start, 1);

    public TextSpan SecondQuestionSpan => new(Span.Start + 1, 1);
}
