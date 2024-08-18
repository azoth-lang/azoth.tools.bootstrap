namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

public sealed class ChildAtVariableSelectorSyntax : SelectorSyntax
{
    public string Child { get; }
    public string Variable { get; }

    public ChildAtVariableSelectorSyntax(string child, string variable)
    {
        Child = child;
        Variable = variable;
    }

    public override string ToString() => $"{Child}[{Variable}]";
}
