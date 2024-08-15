using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class OptionalPatternSyntax : CodeSyntax, IOptionalPatternSyntax
{
    public IOptionalOrBindingPatternSyntax Pattern { get; }

    public OptionalPatternSyntax(TextSpan span, IOptionalOrBindingPatternSyntax pattern)
        : base(span)
    {
        Pattern = pattern;
    }

    public override string ToString() => $"{Pattern}?";
}
