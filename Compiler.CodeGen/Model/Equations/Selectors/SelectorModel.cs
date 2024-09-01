using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;

[Closed(
    typeof(AllChildrenSelectorModel),
    typeof(NamedChildSelectorModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
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
    public bool IsBroadcast { get; }
    public virtual bool IsAllDescendants => false;

    protected SelectorModel(bool isBroadcast)
    {
        IsBroadcast = isBroadcast;
    }

    /// <summary>
    /// Whether this selector matches the given child attribute.
    /// </summary>
    public abstract bool MatchesChild(AttributeModel attribute);

    public sealed override string ToString()
        => IsBroadcast ? $"{ToChildSelectorString()}.**" : ToChildSelectorString();

    protected abstract string ToChildSelectorString();
}
