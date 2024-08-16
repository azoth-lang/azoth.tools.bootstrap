using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;

[Closed(typeof(AspectAttributeSyntax), typeof(PropertySyntax))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeSyntax
{
    public string Name { get; }

    protected AttributeSyntax(string name)
    {
        Name = name;
    }

    public abstract override string ToString();
}
