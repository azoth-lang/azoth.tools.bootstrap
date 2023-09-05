using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

internal abstract class Token
{
    public TextSpan Span { get; }

    protected Token(TextSpan span)
    {
        Span = span;
    }
    public string Text(CodeText code) => Span.GetText(code.Text);
}
