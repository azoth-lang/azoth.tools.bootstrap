using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class ChildListSelectorModel : NamedChildSelectorModel
{
    public override SelectorSyntax Syntax { get; }

    public ChildListSelectorModel(ChildListSelectorSyntax syntax)
        : base(syntax.Child, syntax.Broadcast)
    {
        Syntax = syntax;
    }

    protected override string ToChildSelectorString() => $"{Child}[*]";
}
