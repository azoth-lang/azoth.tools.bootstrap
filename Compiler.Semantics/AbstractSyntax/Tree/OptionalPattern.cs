using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class OptionalPattern : Pattern, IOptionalPattern
{
    public IPattern Pattern { get; }
    public OptionalPattern(TextSpan span, IPattern pattern)
        : base(span)
    {
        Pattern = pattern;
    }

    public override string ToString() => $"({Pattern})?";
}
