using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class SynthesizedAttributeEquationSyntax : EquationSyntax
{
    public EvaluationStrategy? Strategy { get; }

    public SynthesizedAttributeEquationSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        string? expression)
        : base(node, name, isMethod, expression)
    {
        Strategy = strategy;
    }

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {Node}.{Name}{parameters}";
    }
}
