namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class SynthesizedAttributeSyntax : AttributeSyntax
{
    public SymbolSyntax Node { get; }
    public string Attribute { get; }
    public string? Parameters { get; }

    public SynthesizedAttributeSyntax(SymbolSyntax node, string attribute, string? parameters)
    {
        Node = node;
        Attribute = attribute;
        Parameters = parameters;
    }
}
