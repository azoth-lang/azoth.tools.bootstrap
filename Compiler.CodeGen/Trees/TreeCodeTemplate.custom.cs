using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class TreeCodeTemplate
{
    private readonly GrammarNode grammar;

    public TreeCodeTemplate(GrammarNode grammar)
    {
        this.grammar = grammar;
    }
}
