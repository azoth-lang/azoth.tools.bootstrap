using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class TreeCodeTemplate
{
    private readonly Grammar grammar;

    public TreeCodeTemplate(Grammar grammar)
    {
        this.grammar = grammar;
    }
}
