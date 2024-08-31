namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;

public sealed class ChildAtIndexSelectorSyntax : SelectorSyntax
{
    public string Child { get; }
    public int Index { get; }

    public ChildAtIndexSelectorSyntax(string child, int index, bool broadcast)
        : base(broadcast)
    {
        Child = child;
        Index = index;
    }

    public override string ToString()
    {
        var broadcast = Broadcast ? ".**" : "";
        return $"{Child}[{Index}]{broadcast}";
    }
}
