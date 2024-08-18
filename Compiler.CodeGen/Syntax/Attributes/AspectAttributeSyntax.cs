using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[Closed(typeof(SynthesizedAttributeSyntax), typeof(InheritedAttributeSyntax))]
public abstract class AspectAttributeSyntax : AttributeSyntax
{
    public SymbolSyntax Node { get; }

    protected AspectAttributeSyntax(SymbolSyntax node, string name)
        : base(name)
    {
        Node = node;
    }
}
