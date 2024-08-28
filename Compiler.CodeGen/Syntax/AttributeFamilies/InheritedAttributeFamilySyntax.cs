using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class InheritedAttributeFamilySyntax : AttributeFamilySyntax
{
    public InheritedAttributeFamilySyntax(string name, TypeSyntax type)
        : base(name, type)
    {
    }

    public override string ToString() => $"↓ *.{Name} <: {Type}";
}
