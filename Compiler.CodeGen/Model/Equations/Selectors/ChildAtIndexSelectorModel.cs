using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;

public sealed class ChildAtIndexSelectorModel : NamedChildSelectorModel
{
    public override ChildAtIndexSelectorSyntax Syntax { get; }
    public int Index => Syntax.Index;

    public ChildAtIndexSelectorModel(ChildAtIndexSelectorSyntax syntax)
        : base(syntax.Child, syntax.Broadcast)
    {
        Syntax = syntax;
    }

    protected override bool CoversRangeOf(NamedChildSelectorModel selector)
        => selector is ChildAtIndexSelectorModel { Index: var otherIndex } && Index == otherIndex;

    protected override string ToChildSelectorString() => $"{Child}[{Index}]";
}
