using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class ChildAtVariableSelectorModel : SelectorModel
{
    public override ChildAtVariableSelectorSyntax Syntax { get; }
    public string Child => Syntax.Child;
    public string Variable => Syntax.Variable;

    public ChildAtVariableSelectorModel(ChildAtVariableSelectorSyntax syntax)
    {
        Syntax = syntax;
    }
}
