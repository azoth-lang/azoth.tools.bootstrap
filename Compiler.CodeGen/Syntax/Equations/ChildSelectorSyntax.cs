namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class ChildSelectorSyntax : SelectorSyntax
{
    public string Child { get; }

    public ChildSelectorSyntax(string child, bool broadcast)
        : base(broadcast)
    {
        Child = child;
    }

    public override string ToString() => Broadcast ? $"{Child}.**" : Child;
}
