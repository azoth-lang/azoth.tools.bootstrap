using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public partial class NodeCodeTemplate
{
    private readonly Language language;
    private readonly Grammar grammar;

    public NodeCodeTemplate(Language language)
    {
        this.language = language;
        grammar = language.Grammar;
    }
}
