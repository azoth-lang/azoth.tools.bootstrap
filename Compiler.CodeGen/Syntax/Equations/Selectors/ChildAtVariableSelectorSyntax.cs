namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;

public sealed class ChildAtVariableSelectorSyntax : SelectorSyntax
{
    public string Child { get; }
    public string Variable { get; }

    public ChildAtVariableSelectorSyntax(string child, string variable, bool broadcast)
        : base(broadcast)
    {
        Child = child;
        Variable = variable;
    }

    public override string ToString()
    {
        var broadcast = Broadcast ? ".**" : "";
        return $"{Child}[{Variable}]{broadcast}";
    }
}
