using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
internal abstract class Syntax : IConcreteSyntax
{
    public TextSpan Span { get; protected set; }

    protected Syntax(TextSpan span)
    {
        Span = span;
    }

    // This exists primarily for debugging use
    public abstract override string ToString();
}
