using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class TreeCodeTemplate
{
    private readonly Language language;
    private readonly Grammar grammar;

    public TreeCodeTemplate(Language language)
    {
        this.language = language;
        grammar = language.Grammar;
    }
}
