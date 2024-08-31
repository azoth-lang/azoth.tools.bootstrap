using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class IntertypeMethodAttributeSyntax : AspectAttributeSyntax
{
    /// <remarks>For the moment, parameters are a single string rather than parsed. This prevents
    /// code gen from calling aspect methods though.</remarks>
    public string Parameters { get; }
    public override TypeSyntax Type { get; }
    public string? DefaultExpression { get; }

    public IntertypeMethodAttributeSyntax(SymbolSyntax node, string name, string parameters, TypeSyntax type, string? defaultExpression)
        : base(false, EvaluationStrategy.Computed, node, name, true)
    {
        Parameters = parameters;
        DefaultExpression = defaultExpression;
        Type = type;
    }

    public override string ToString()
    {
        var defaultExpression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"+ {Node}.{Name}({Parameters}): {Type}{defaultExpression};";
    }
}
