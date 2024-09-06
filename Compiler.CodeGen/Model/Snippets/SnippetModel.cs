using System;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Snippets;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Snippets;

[Closed(typeof(ConstructorArgumentValidationModel))]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public abstract class SnippetModel
{
    public static SnippetModel Create(AspectModel aspect, SnippetSyntax syntax)
        => syntax switch
        {
            ConstructorArgumentValidationSyntax s => new ConstructorArgumentValidationModel(aspect, s),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    public AspectModel Aspect { get; }
    public abstract SnippetSyntax Syntax { get; }
    public InternalSymbol? NodeSymbol { get; set; }
    public TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;

    protected SnippetModel(AspectModel aspect, SymbolSyntax node)
    {
        Aspect = aspect;
        NodeSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, node);
        this.node = new(() => Aspect.Tree.NodeFor(NodeSymbol)
                          ?? throw new($"Snippet '{this}' refers to node '{NodeSymbol}' that does not exist."));
    }

    public abstract override string ToString();
}
