namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class PropertySyntax : AttributeSyntax
{
    public PropertySyntax(string name, TypeSyntax type)
        : base(name, type)
    {
    }

    public override string ToString() => $"{Name}:{Type}";
}
