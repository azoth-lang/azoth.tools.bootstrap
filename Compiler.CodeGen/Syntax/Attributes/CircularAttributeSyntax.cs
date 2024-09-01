using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class CircularAttributeSyntax : AspectAttributeSyntax
{
    public override TypeSyntax Type { get; }
    public string? DefaultExpression { get; }
    public string? InitialExpression { get; }

    public CircularAttributeSyntax(
        SymbolSyntax node,
        TypeSyntax type,
        string name,
        string? defaultExpression,
        string? initialExpression)
        : base(false, EvaluationStrategy.Lazy, node, name, false)
    {
        Type = type;
        DefaultExpression = defaultExpression;
        InitialExpression = initialExpression;
    }

    public override string ToString()
    {
        var defaultExpression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        var initialExpression = InitialExpression is not null ? $" initial => {InitialExpression}" : "";
        return $"⟳ {Node}.{Name}: {Type}{defaultExpression}{initialExpression}";
    }
}
