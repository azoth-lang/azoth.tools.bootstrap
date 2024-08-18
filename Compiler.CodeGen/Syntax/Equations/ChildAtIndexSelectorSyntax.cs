namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class ChildAtIndexSelectorSyntax : SelectorSyntax
{
    public string Child { get; }
    public int Index { get; }

    public ChildAtIndexSelectorSyntax(string child, int index)
    {
        Child = child;
        Index = index;
    }

    public override string ToString() => $"{Child}[{Index}]";
}
