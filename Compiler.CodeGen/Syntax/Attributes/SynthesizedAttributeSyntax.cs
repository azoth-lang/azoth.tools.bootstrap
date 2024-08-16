using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class SynthesizedAttributeSyntax : AspectAttributeSyntax
{
    public EvaluationStrategy? Strategy { get; }
    public string? Parameters { get; }
    public TypeSyntax Type { get; }
    public string? DefaultExpression { get; }

    public SynthesizedAttributeSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        string? parameters,
        TypeSyntax type,
        string? defaultExpression)
        : base(node, name)
    {
        Strategy = strategy;
        Parameters = parameters;
        Type = type;
        DefaultExpression = defaultExpression;
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        if (strategy.Length > 0)
            strategy += " ";
        var expression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"↑ {strategy}{Node}.{Name}{Parameters}: {Type}{expression};";
    }
}
