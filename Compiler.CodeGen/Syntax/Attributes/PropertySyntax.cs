using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

public sealed class PropertySyntax : TreeAttributeSyntax
{
    public override TypeSyntax Type { get; }

    public PropertySyntax(string name, TypeSyntax type)
        : base(name)
    {
        Type = type;
    }

    public override string ToString() => $"{Name}:{Type}";
}
