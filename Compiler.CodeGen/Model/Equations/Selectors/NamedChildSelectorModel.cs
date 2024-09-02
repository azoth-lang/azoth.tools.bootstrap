using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;

[Closed(
    typeof(ChildSelectorModel),
    typeof(ChildAtIndexSelectorModel),
    typeof(ChildAtVariableSelectorModel),
    typeof(ChildListSelectorModel))]
public abstract class NamedChildSelectorModel : SelectorModel
{
    public string Child { get; }

    protected NamedChildSelectorModel(string child, bool isBroadcast)
        : base(isBroadcast)
    {
        Child = child;
    }

    public override bool Matches(AttributeModel attribute)
        => attribute.Name == Child;
}
