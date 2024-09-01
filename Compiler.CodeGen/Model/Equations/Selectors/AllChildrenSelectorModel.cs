using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;

public sealed class AllChildrenSelectorModel : SelectorModel
{
    public static AllChildrenSelectorModel Create(AllChildrenSelectorSyntax syntax)
        => syntax == AllChildrenSelectorSyntax.Instance ? Instance : BroadcastInstance;

    public static AllChildrenSelectorModel Instance { get; } = new(AllChildrenSelectorSyntax.Instance);
    public static AllChildrenSelectorModel BroadcastInstance { get; } = new(AllChildrenSelectorSyntax.BroadcastInstance);

    public override AllChildrenSelectorSyntax Syntax { get; }
    public override bool IsAllDescendants => IsBroadcast;

    private AllChildrenSelectorModel(AllChildrenSelectorSyntax syntax)
        : base(syntax.Broadcast)
    {
        Syntax = syntax;
    }

    public override bool MatchesChild(AttributeModel attribute) => attribute.IsChild;

    protected override string ToChildSelectorString() => "*";
}
