using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class TreeCodeTemplate
{
    private readonly Grammar grammar;

    public TreeCodeTemplate(Grammar grammar)
    {
        this.grammar = grammar;
    }
}
