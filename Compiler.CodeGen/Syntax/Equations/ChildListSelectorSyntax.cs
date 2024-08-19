namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class ChildListSelectorSyntax : SelectorSyntax
{
    public string Child { get; }

    public ChildListSelectorSyntax(string child, bool broadcast)
        : base(broadcast)
    {
        Child = child;
    }

    public override string ToString()
    {
        var broadcast = Broadcast ? ".**" : "";
        return $"{Child}[*]{broadcast}";
    }
}
