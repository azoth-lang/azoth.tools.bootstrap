namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class RewriteRuleSyntax
{
    public SymbolSyntax Node { get; }
    public string? Name { get; }

    public RewriteRuleSyntax(SymbolSyntax node, string? name)
    {
        Node = node;
        Name = name;
    }

    public override string ToString()
        => Name is not null ? $"✎ {Node} {Name}" : $"✎ {Node}";
}
