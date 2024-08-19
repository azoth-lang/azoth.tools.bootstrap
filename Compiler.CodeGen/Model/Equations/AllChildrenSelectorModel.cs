using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class AllChildrenSelectorModel : SelectorModel
{
    public static AllChildrenSelectorModel Create(AllChildrenSelectorSyntax syntax)
        => syntax == AllChildrenSelectorSyntax.Instance ? Instance : BroadcastInstance;

    public static AllChildrenSelectorModel Instance { get; } = new(AllChildrenSelectorSyntax.Instance);
    public static AllChildrenSelectorModel BroadcastInstance { get; } = new(AllChildrenSelectorSyntax.BroadcastInstance);


    private AllChildrenSelectorModel(AllChildrenSelectorSyntax syntax)
        : base(syntax.Broadcast)
    {
        Syntax = syntax;
    }

    public override AllChildrenSelectorSyntax Syntax { get; }
    public override bool IsAllDescendants => Broadcast;
}
