using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations.Selectors;


namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;

public sealed class ChildSelectorModel : NamedChildSelectorModel
{
    public override ChildSelectorSyntax Syntax { get; }

    public ChildSelectorModel(ChildSelectorSyntax syntax)
        : base(syntax.Child, syntax.Broadcast)
    {
        Syntax = syntax;
    }

    protected override string ToChildSelectorString() => Child;
}
