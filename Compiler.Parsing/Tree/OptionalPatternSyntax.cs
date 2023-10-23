using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class OptionalPatternSyntax : Syntax, IOptionalPatternSyntax
{
    public IPatternSyntax Pattern { get; }

    public OptionalPatternSyntax(TextSpan span, IPatternSyntax pattern)
        : base(span)
    {
        Pattern = pattern;
    }

    public override string ToString() => $"({Pattern})?";
}
