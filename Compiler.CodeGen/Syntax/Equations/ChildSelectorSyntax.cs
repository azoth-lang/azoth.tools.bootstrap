namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class ChildSelectorSyntax : SelectorSyntax
{
    public string Child { get; }

    public ChildSelectorSyntax(string child)
    {
        Child = child;
    }

    public override string ToString() => Child;
}
