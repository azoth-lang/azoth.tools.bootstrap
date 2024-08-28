using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeKins;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class InheritedAttributeKinSyntax : AttributeKinSyntax
{
    public InheritedAttributeKinSyntax(string name, TypeSyntax type)
        : base(name, type)
    {
    }

    public override string ToString() => $"↓ *.{Name} <: {Type}";
}
