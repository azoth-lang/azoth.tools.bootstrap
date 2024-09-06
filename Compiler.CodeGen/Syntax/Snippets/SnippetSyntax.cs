using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Snippets;

[Closed(typeof(ConstructorArgumentValidationSyntax))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class SnippetSyntax
{
    public SymbolSyntax Node { get; }

    protected SnippetSyntax(SymbolSyntax node)
    {
        Node = node;
    }

    public abstract override string ToString();
}
