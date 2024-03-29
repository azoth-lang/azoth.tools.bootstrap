using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class OptionalPatternSyntax : Syntax, IOptionalPatternSyntax
{
    public IOptionalOrBindingPatternSyntax Pattern { get; }

    public OptionalPatternSyntax(TextSpan span, IOptionalOrBindingPatternSyntax pattern)
        : base(span)
    {
        Pattern = pattern;
    }

    public override string ToString() => $"{Pattern}?";
}
