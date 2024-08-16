namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class PropertySyntax : AttributeSyntax
{
    public TypeSyntax Type { get; init; }

    public PropertySyntax(string name, TypeSyntax type)
        : base(name)
    {
        Type = type;
    }

    public override string ToString() => $"{Name}:{Type}";
}
