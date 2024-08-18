namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class SynthesizedAttributeEquationSyntax : EquationSyntax
{
    public EvaluationStrategy? Strategy { get; }
    public string Name { get; }
    public string? Parameters { get; }
    public TypeSyntax? TypeOverride { get; }
    public string? Expression { get; }

    public SynthesizedAttributeEquationSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        string? parameters,
        TypeSyntax? typeOverride,
        string? expression)
        : base(node)
    {
        Strategy = strategy;
        Name = name;
        Parameters = parameters;
        TypeOverride = typeOverride;
        Expression = expression;
    }
}
