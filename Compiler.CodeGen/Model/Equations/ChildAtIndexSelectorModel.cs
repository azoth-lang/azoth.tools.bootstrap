using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class ChildAtIndexSelectorModel : NamedChildSelectorModel
{
    public override ChildAtIndexSelectorSyntax Syntax { get; }
    public int Index => Syntax.Index;

    public ChildAtIndexSelectorModel(ChildAtIndexSelectorSyntax syntax)
        : base(syntax.Child, syntax.Broadcast)
    {
        Syntax = syntax;
    }

    protected override string ToChildSelectorString() => $"{Child}[{Index}]";
}
