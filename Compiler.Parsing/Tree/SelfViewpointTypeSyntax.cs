using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class SelfViewpointTypeSyntax : TypeSyntax, ISelfViewpointTypeSyntax
{
    public ITypeSyntax Referent { get; }

    public SelfViewpointTypeSyntax(TextSpan span, ITypeSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    public override string ToString() => $"self|>{Referent}";
}
