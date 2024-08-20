namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class SynthesizedAttributeEquationSyntax : EquationSyntax
{
    public EvaluationStrategy? Strategy { get; }
    public TypeSyntax? TypeOverride { get; }

    public SynthesizedAttributeEquationSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        TypeSyntax? typeOverride,
        string? expression)
        : base(node, name, isMethod, expression)
    {
        Strategy = strategy;
        TypeOverride = typeOverride;
    }
}
