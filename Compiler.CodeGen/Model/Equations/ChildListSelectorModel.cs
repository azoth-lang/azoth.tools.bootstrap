using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class ChildListSelectorModel : SelectorModel
{
    public override SelectorSyntax Syntax { get; }
    public string Child { get; }

    public ChildListSelectorModel(ChildListSelectorSyntax syntax)
        : base(syntax.Broadcast)
    {
        Syntax = syntax;
        Child = syntax.Child;
    }

    public override string ToString()
    {
        var broadcast = Broadcast ? ".**" : "";
        return $"{Child}[*]{broadcast}";
    }
}
