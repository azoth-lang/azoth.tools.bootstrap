using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class ChildrenCodeTemplate
{
    private readonly Grammar grammar;

    public ChildrenCodeTemplate(Grammar grammar)
    {
        this.grammar = grammar;
    }
}
