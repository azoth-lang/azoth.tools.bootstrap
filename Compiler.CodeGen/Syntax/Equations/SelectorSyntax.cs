using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

[Closed(
    typeof(AllChildrenSelectorSyntax),
    typeof(DescendantsSelectorSyntax),
    typeof(ChildSelectorSyntax),
    typeof(ChildAtIndexSelectorSyntax),
    typeof(ChildAtVariableSelectorSyntax))]
public abstract class SelectorSyntax
{
    public abstract override string ToString();
}
