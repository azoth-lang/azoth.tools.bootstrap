using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(
    typeof(AllChildrenSelectorModel),
    typeof(ChildSelectorModel),
    typeof(ChildAtIndexSelectorModel),
    typeof(ChildAtVariableSelectorModel),
    typeof(ChildListSelectorModel))]
public abstract class SelectorModel
{
    public static SelectorModel Create(SelectorSyntax syntax)
        => syntax switch
        {
            AllChildrenSelectorSyntax syn => AllChildrenSelectorModel.Create(syn),
            ChildSelectorSyntax syn => new ChildSelectorModel(syn),
            ChildAtIndexSelectorSyntax syn => new ChildAtIndexSelectorModel(syn),
            ChildAtVariableSelectorSyntax syn => new ChildAtVariableSelectorModel(syn),
            ChildListSelectorSyntax syn => new ChildListSelectorModel(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public abstract SelectorSyntax Syntax { get; }
    /// <summary>
    /// Whether the selector should broadcast to all descendants of the selected node.
    /// </summary>
    public bool Broadcast { get; }
    public virtual bool IsAllDescendants => false;

    protected SelectorModel(bool broadcast)
    {
        Broadcast = broadcast;
    }
}
