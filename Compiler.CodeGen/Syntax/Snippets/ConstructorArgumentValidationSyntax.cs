namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Snippets;

public sealed class ConstructorArgumentValidationSyntax : SnippetSyntax
{
    public ConstructorArgumentValidationSyntax(SymbolSyntax node)
        : base(node)
    {
    }

    public override string ToString() => $"+ {Node}.new.Validate";
}
