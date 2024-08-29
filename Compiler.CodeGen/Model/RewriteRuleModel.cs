using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class RewriteRuleModel
{
    public AspectModel Aspect { get; }

    public RewriteRuleSyntax Syntax { get; }
    public InternalSymbol NodeSymbol { get; }
    public TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;
    public string? Name => Syntax.Name;
    public SymbolTypeModel RewriteToType => rewriteToType.Value;
    private readonly Lazy<SymbolTypeModel> rewriteToType;

    public RewriteRuleModel(AspectModel aspect, RewriteRuleSyntax syntax)
    {
        Aspect = aspect;
        Syntax = syntax;
        NodeSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node);
        node = new(() => aspect.Tree.NodeFor(NodeSymbol)
                         ?? throw new($"Rewrite '{Syntax}' refers to node '{NodeSymbol}' that does not exist."));
        rewriteToType = new(ComputeRewriteToType);
    }

    private SymbolTypeModel ComputeRewriteToType()
    {
        var fromType = Node.DefinesType;
        var rewriteToTypes = Aspect.Tree.Nodes.SelectMany(n => n.TreeChildNodes)
                                   .Select(n => n.DefinesType)
                                   .Where(fromType.IsSubtypeOf)
                                   .MostSpecificTypes()
                                   .ToFixedSet();
        var rewriteToType = rewriteToTypes.TrySingle();
        if (rewriteToType is null)
            throw new($"Rewrite '{this}' does not have a specific rewrite to type. Candidates: {string.Join(", ", rewriteToTypes)}.");
        return rewriteToType;
    }

    public override string ToString()
        => Name is not null ? $"✎ {NodeSymbol} {Name}" : $"✎ {NodeSymbol}";
}
