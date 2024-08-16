using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[Closed(typeof(SynthesizedAttributeSyntax))]
public abstract class AttributeSyntax
{
    public SymbolSyntax Node { get; }
    public string Name { get; }

    protected AttributeSyntax(SymbolSyntax node, string name)
    {
        Node = node;
        Name = name;
    }
}
