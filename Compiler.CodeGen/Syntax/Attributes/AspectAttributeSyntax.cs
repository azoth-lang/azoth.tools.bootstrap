using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[Closed(
    typeof(SynthesizedAttributeSyntax),
    typeof(InheritedAttributeSyntax),
    typeof(PreviousAttributeSyntax),
    typeof(IntertypeMethodAttributeSyntax))]
public abstract class AspectAttributeSyntax : AttributeSyntax
{
    public EvaluationStrategy? Strategy { get; }
    public SymbolSyntax Node { get; }
    public bool IsMethod { get; }

    protected AspectAttributeSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        TypeSyntax type)
        : base(name, type)
    {
        Strategy = strategy;
        Node = node;
        IsMethod = isMethod;
    }
}
