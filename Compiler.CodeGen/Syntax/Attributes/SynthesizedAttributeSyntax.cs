using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class SynthesizedAttributeSyntax : AspectAttributeSyntax
{
    public string? DefaultExpression { get; }

    public SynthesizedAttributeSyntax(
        EvaluationStrategy? strategy,
        SymbolSyntax node,
        string name,
        bool isMethod,
        TypeSyntax type,
        string? defaultExpression)
        : base(strategy, node, name, isMethod, type)
    {
        DefaultExpression = defaultExpression;
    }

    public override string ToString()
    {
        var strategy = Strategy.ToSourceString();
        if (strategy.Length > 0)
            strategy += " ";
        var parameters = IsMethod ? "()" : "";
        var expression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"↑ {strategy}{Node}.{Name}{parameters}: {Type}{expression};";
    }
}
