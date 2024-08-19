using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class InheritedAttributeSupertypeSyntax
{
    public string Name { get; }
    public TypeSyntax Type { get; }

    public InheritedAttributeSupertypeSyntax(string name, TypeSyntax type)
    {
        Name = name;
        Type = type;
    }

    public override string ToString() => $"↓ *.{Name} <: {Type}";
}
