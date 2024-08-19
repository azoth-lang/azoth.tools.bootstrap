using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class ChildSelectorModel : SelectorModel
{
    public override ChildSelectorSyntax Syntax { get; }
    public string Child => Syntax.Child;

    public ChildSelectorModel(ChildSelectorSyntax syntax)
        : base(syntax.Broadcast)
    {
        Syntax = syntax;
    }
}
