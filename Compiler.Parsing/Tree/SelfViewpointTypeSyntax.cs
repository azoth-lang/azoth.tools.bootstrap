using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class SelfViewpointTypeSyntax : TypeSyntax, ISelfViewpointTypeSyntax
{
    public ITypeSyntax Referent { get; }
    public Promise<SelfParameterSymbol?> ReferencedSymbol { get; } = new();

    public SelfViewpointTypeSyntax(TextSpan span, ITypeSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    public override string ToString() => $"self|>{Referent}";
}
