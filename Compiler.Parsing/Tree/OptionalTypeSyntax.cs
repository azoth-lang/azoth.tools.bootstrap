using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class OptionalTypeSyntax : TypeSyntax, IOptionalTypeSyntax
{
    public ITypeSyntax Referent { get; }

    public OptionalTypeSyntax(TextSpan span, ITypeSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    public override string ToString() => $"{Referent}?";
}
