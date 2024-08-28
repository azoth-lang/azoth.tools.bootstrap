using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeKins;

[Closed(typeof(InheritedAttributeKinSyntax), typeof(AggregateAttributeKinSyntax))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeKinSyntax
{
    public string Name { get; }
    public TypeSyntax Type { get; }

    protected AttributeKinSyntax(string name, TypeSyntax type)
    {
        Name = name;
        Type = type;
    }

    public abstract override string ToString();
}
