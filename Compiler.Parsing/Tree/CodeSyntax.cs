using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
internal abstract class CodeSyntax : ICodeSyntax
{
    public TextSpan Span { get; }

    protected CodeSyntax(TextSpan span)
    {
        Span = span;
    }

    // This exists primarily for debugging use
    public abstract override string ToString();
}
