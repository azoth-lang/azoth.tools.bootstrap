namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class AllChildrenSelectorSyntax : SelectorSyntax
{
    public static AllChildrenSelectorSyntax Create(bool broadcast) => broadcast ? BroadcastInstance : Instance;

    public static AllChildrenSelectorSyntax Instance { get; } = new(false);
    public static AllChildrenSelectorSyntax BroadcastInstance { get; } = new(true);

    private AllChildrenSelectorSyntax(bool broadcast) : base(broadcast) { }

    public override string ToString() => Broadcast ? "*.**" : "*";
}
