using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class ChildAtIndexSelectorModel : SelectorModel
{
    public override ChildAtIndexSelectorSyntax Syntax { get; }
    public string Child => Syntax.Child;
    public int Index => Syntax.Index;

    public ChildAtIndexSelectorModel(ChildAtIndexSelectorSyntax syntax)
        : base(syntax.Broadcast)
    {
        Syntax = syntax;
    }
}
