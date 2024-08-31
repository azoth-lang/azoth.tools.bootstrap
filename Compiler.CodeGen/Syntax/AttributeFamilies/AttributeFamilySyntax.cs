using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

[Closed(
    typeof(InheritedAttributeFamilySyntax),
    typeof(PreviousAttributeFamilySyntax),
    typeof(AggregateAttributeFamilySyntax))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class AttributeFamilySyntax
{
    public string Name { get; }
    public TypeSyntax Type { get; }

    protected AttributeFamilySyntax(string name, TypeSyntax type)
    {
        Name = name;
        Type = type;
    }

    public abstract override string ToString();
}
