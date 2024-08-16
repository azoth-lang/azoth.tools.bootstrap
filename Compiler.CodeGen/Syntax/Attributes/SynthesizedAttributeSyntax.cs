namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class SynthesizedAttributeSyntax : AttributeSyntax
{
    public SymbolSyntax Node { get; }
    public string Name { get; }
    public string? Parameters { get; }
    public TypeSyntax Type { get; }
    public string? DefaultExpression { get; }

    public SynthesizedAttributeSyntax(
        SymbolSyntax node,
        string name,
        string? parameters,
        TypeSyntax type,
        string? defaultExpression)
    {
        Node = node;
        Name = name;
        Parameters = parameters;
        Type = type;
        DefaultExpression = defaultExpression;
    }
}
