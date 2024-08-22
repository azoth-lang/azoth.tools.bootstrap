using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[Closed(typeof(AspectAttributeSyntax), typeof(PropertySyntax))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeSyntax
{
    public string Name { get; }
    public TypeSyntax Type { get; }

    protected AttributeSyntax(string name, TypeSyntax type)
    {
        Name = name;
        Type = type;
    }

    public abstract override string ToString();
}
