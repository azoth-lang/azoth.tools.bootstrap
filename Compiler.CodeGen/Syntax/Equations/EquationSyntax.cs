using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

[Closed(
    typeof(SynthesizedAttributeEquationSyntax),
    typeof(InheritedAttributeEquationSyntax),
    typeof(IntertypeMethodEquationSyntax),
    typeof(AggregateAttributeEquationSyntax))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class EquationSyntax
{
    public SymbolSyntax Node { get; }
    public string Name { get; }
    public bool IsMethod { get; }
    public string? Expression { get; }

    protected EquationSyntax(SymbolSyntax node, string name, bool isMethod, string? expression)
    {
        Node = node;
        Name = name;
        IsMethod = isMethod;
        Expression = expression;
    }

    public abstract override string ToString();
}
