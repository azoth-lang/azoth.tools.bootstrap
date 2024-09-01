using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class LocalAttributeEquationSyntax : EquationSyntax
{
    public EvaluationStrategy? Strategy { get; }

    public LocalAttributeEquationSyntax(
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
