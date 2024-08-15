using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;

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
