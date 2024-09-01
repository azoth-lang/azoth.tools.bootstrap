using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[Closed(
    typeof(SynthesizedAttributeSyntax),
    typeof(CircularAttributeSyntax),
    typeof(InheritedAttributeSyntax),
    typeof(PreviousAttributeSyntax),
    typeof(IntertypeMethodAttributeSyntax),
    typeof(AggregateAttributeSyntax))]
public abstract class AspectAttributeSyntax : AttributeSyntax
{
    public bool IsChild { get; }
    public EvaluationStrategy? Strategy { get; }
    public SymbolSyntax Node { get; }
    public bool IsMethod { get; }

    protected AspectAttributeSyntax(
        bool isChild,
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod)
        : base(name)
    {
        IsChild = isChild;
        Strategy = strategy;
        Node = node;
        IsMethod = isMethod;
    }
}
