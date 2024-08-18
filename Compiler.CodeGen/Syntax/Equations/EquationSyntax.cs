using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

[Closed(typeof(SynthesizedAttributeEquationSyntax), typeof(InheritedAttributeEquationSyntax))]
public abstract class EquationSyntax
{
    public SymbolSyntax Node { get; }

    protected EquationSyntax(SymbolSyntax node)
    {
        Node = node;
    }
}
