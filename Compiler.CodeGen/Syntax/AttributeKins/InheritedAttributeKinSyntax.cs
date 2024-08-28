using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeKins;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class InheritedAttributeKinSyntax
{
    public string Name { get; }
    public TypeSyntax Type { get; }

    public InheritedAttributeKinSyntax(string name, TypeSyntax type)
    {
        Name = name;
        Type = type;
    }

    public override string ToString() => $"↓ *.{Name} <: {Type}";
}
