using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class Attribute : AbstractSyntax, IAttribute
{
    // TODO this needs type arguments too right?
    public TypeSymbol ReferencedSymbol { get; }

    public Attribute(TextSpan span, TypeSymbol referencedSymbol)
        : base(span)
    {
        ReferencedSymbol = referencedSymbol;
    }

    // TODO this needs type arguments too right?
    public override string ToString() => $"#{ReferencedSymbol}";
}
