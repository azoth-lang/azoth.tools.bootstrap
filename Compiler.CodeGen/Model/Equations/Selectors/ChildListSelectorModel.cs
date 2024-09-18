using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;

public sealed class ChildListSelectorModel : NamedChildSelectorModel
{
    public override SelectorSyntax Syntax { get; }

    public ChildListSelectorModel(ChildListSelectorSyntax syntax)
        : base(syntax.Child, syntax.Broadcast)
    {
        Syntax = syntax;
    }

    protected override bool CoversRangeOf(NamedChildSelectorModel selector) => true;

    protected override string ToChildSelectorString() => $"{Child}[*]";
}
