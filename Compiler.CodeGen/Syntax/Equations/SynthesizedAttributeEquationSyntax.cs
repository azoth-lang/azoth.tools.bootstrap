namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class SynthesizedAttributeEquationSyntax : EquationSyntax
{
    public EvaluationStrategy? EvaluationStrategy { get; }
    public string Name { get; }
    public string? Parameters { get; }
    public TypeSyntax? TypeOverride { get; }
    public string? Expression { get; }

    public SynthesizedAttributeEquationSyntax(
        EvaluationStrategy? evaluationStrategy,
        SymbolSyntax node,
        string name,
        string? parameters,
        TypeSyntax? typeOverride,
        string? expression)
        : base(node)
    {
        EvaluationStrategy = evaluationStrategy;
        Name = name;
        Parameters = parameters;
        TypeOverride = typeOverride;
        Expression = expression;
    }
}
