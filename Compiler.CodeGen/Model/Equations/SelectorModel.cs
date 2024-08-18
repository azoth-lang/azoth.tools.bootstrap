using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

[Closed(
    typeof(AllChildrenSelectorModel),
    typeof(DescendantsSelectorModel),
    typeof(ChildSelectorModel),
    typeof(ChildAtIndexSelectorModel),
    typeof(ChildAtVariableSelectorModel))]
public abstract class SelectorModel
{
    public static SelectorModel Create(SelectorSyntax syntax)
        => syntax switch
        {
            AllChildrenSelectorSyntax _ => AllChildrenSelectorModel.Instance,
            DescendantsSelectorSyntax _ => DescendantsSelectorModel.Instance,
            ChildSelectorSyntax syn => new ChildSelectorModel(syn),
            ChildAtIndexSelectorSyntax syn => new ChildAtIndexSelectorModel(syn),
            ChildAtVariableSelectorSyntax syn => new ChildAtVariableSelectorModel(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public abstract SelectorSyntax Syntax { get; }
    public virtual bool IsAllDescendants => false;
}
