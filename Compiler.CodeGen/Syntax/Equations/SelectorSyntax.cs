using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

[Closed(
    typeof(AllChildrenSelectorSyntax),
    typeof(ChildSelectorSyntax),
    typeof(ChildAtIndexSelectorSyntax),
    typeof(ChildAtVariableSelectorSyntax),
    typeof(ChildListSelectorSyntax))]
public abstract class SelectorSyntax
{
    /// <summary>
    /// Whether the selector should broadcast to all descendants of the selected node.
    /// </summary>
    public bool Broadcast { get; }

    protected SelectorSyntax(bool broadcast)
    {
        Broadcast = broadcast;
    }

    public abstract override string ToString();
}
