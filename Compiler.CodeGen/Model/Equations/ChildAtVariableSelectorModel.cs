using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class ChildAtVariableSelectorModel : NamedChildSelectorModel
{
    public override ChildAtVariableSelectorSyntax Syntax { get; }
    public string Variable => Syntax.Variable;

    public ChildAtVariableSelectorModel(ChildAtVariableSelectorSyntax syntax)
        : base(syntax.Child, syntax.Broadcast)
    {
        Syntax = syntax;
    }

    protected override string ToChildSelectorString() => $"{Child}[{Variable}]";
}
