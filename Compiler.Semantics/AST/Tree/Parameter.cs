using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class Parameter : AbstractSyntax, IParameter
{
    public bool Unused { get; }

    protected Parameter(TextSpan span, bool unused)
        : base(span)
    {
        Unused = unused;
    }
}
