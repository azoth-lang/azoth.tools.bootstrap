using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class ChildrenCodeTemplate
{
    private readonly Language language;
    private readonly Grammar grammar;

    public ChildrenCodeTemplate(Language language)
    {
        this.language = language;
        grammar = language.Grammar;
    }
}
