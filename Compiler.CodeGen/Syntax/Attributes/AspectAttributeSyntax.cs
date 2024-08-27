using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[Closed(
    typeof(SynthesizedAttributeSyntax),
    typeof(InheritedAttributeSyntax),
    typeof(PreviousAttributeSyntax),
    typeof(IntertypeMethodAttributeSyntax))]
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
        bool isMethod,
        TypeSyntax type)
        : base(name, type)
    {
        IsChild = isChild;
        Strategy = strategy;
        Node = node;
        IsMethod = isMethod;
    }
}
