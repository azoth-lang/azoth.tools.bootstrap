using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal abstract class Pattern : AbstractSyntax, IPattern
{
    protected Pattern(TextSpan span)
        : base(span) { }
}
