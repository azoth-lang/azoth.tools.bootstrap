using System;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class RewriteRuleModel
{
    public AspectModel Aspect { get; }

    public RewriteRuleSyntax Syntax { get; }
    public InternalSymbol NodeSymbol { get; }
    public TreeNodeModel Node => node.Value;
    private readonly Lazy<TreeNodeModel> node;
    public RewriteKind Kind => Syntax.Kind;
    public InternalSymbol? ToNodeSymbol { get; }
    public TreeNodeModel? ToNode => toNode.Value;
    private readonly Lazy<TreeNodeModel?> toNode;
    public string? Name => Syntax.Name;
    public SymbolTypeModel RewriteToType => rewriteToType.Value;
    private readonly Lazy<SymbolTypeModel> rewriteToType;

    public RewriteRuleModel(AspectModel aspect, RewriteRuleSyntax syntax)
    {
        Aspect = aspect;
        Syntax = syntax;
        NodeSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node);
        node = new(() => aspect.Tree.NodeFor(NodeSymbol)
                         ?? throw CreateLookupFailed(NodeSymbol));
        ToNodeSymbol = Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.ToNode);
        toNode = new(() => ToNodeSymbol is { } symbol
            ? aspect.Tree.NodeFor(symbol) ?? throw CreateLookupFailed(symbol) : null);

        rewriteToType = new(ComputeRewriteToType);
    }

    private Exception CreateLookupFailed(InternalSymbol symbol)
        => new($"Rewrite '{Syntax}' refers to node '{symbol}' that does not exist.");

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

        if (ToNode is not { } toNode)
            return rewriteToType;

        var toType = toNode.DefinesType;
        if (!toType.IsSubtypeOf(rewriteToType))
            throw new($"Rewrite '{this}' specifies rewrite to type {toType} that is not a subtype of the rewrite type {rewriteToType}.");
        return toType;
    }

    public override string ToString()
        => Kind switch
        {
            RewriteKind.InsertAbove => $"✎ {NodeSymbol} insert {ToNode}",
            RewriteKind.Replace => $"✎ {NodeSymbol} replace_with {ToNode}",
            RewriteKind.RewriteSubtree => Name is not null ? $"✎ {NodeSymbol} {Name}"
                : (ToNodeSymbol is not null ? $"✎ {NodeSymbol} rewrite {ToNodeSymbol}" : $"✎ {NodeSymbol} rewrite"),
            _ => throw ExhaustiveMatch.Failed(Kind)
        };
}
