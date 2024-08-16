namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class SynthesizedAttributeSyntax : AspectAttributeSyntax
{
    public string? Parameters { get; }
    public TypeSyntax Type { get; }
    public string? DefaultExpression { get; }

    public SynthesizedAttributeSyntax(
        SymbolSyntax node,
        string name,
        string? parameters,
        TypeSyntax type,
        string? defaultExpression)
        : base(node, name)
    {
        Parameters = parameters;
        Type = type;
        DefaultExpression = defaultExpression;
    }

    public override string ToString()
    {
        var expression = DefaultExpression is not null ? $" => {DefaultExpression}" : "";
        return $"↑ {Node}.{Name}{Parameters}: {Type}{expression};";
    }
}
