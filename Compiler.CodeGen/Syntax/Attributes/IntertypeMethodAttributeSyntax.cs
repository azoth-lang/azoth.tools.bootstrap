using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class IntertypeMethodAttributeSyntax : AspectAttributeSyntax
{
    /// <remarks>For the moment, parameters are a single string rather than parsed. This prevents
    /// code gen from calling aspect methods though.</remarks>
    public string Parameters { get; }
    public string? DefaultExpression { get; }

    public IntertypeMethodAttributeSyntax(SymbolSyntax node, string name, string parameters, TypeSyntax type, string? defaultExpression)
        : base(EvaluationStrategy.Computed, node, name, true, type)
    {
        Parameters = parameters;
        DefaultExpression = defaultExpression;
    }

    public override string ToString()
    {
        var defaultExpression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"+ {Node}.{Name}({Parameters}): {Type}{defaultExpression};";
    }
}
