namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class TypeDeclarationSyntax
{
    public bool IsValueType { get; }
    public SymbolSyntax Name { get; }

    public TypeDeclarationSyntax(bool isValueType, SymbolSyntax name)
    {
        IsValueType = isValueType;
        Name = name;
    }
}
