using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class RewriteRuleSyntax
{
    public SymbolSyntax Node { get; }
    public string? Name { get; }
    public RewriteKind Kind { get; }
    public SymbolSyntax? ToNode { get; }

    public RewriteRuleSyntax(SymbolSyntax node, string? name, RewriteKind kind, SymbolSyntax? toNode)
    {
        Node = node;
        Name = name;
        Kind = kind;
        ToNode = toNode;
    }

    public override string ToString()
    {
        var name = Name is not null ? $" {Name}" : "";
        return Kind switch
        {
            RewriteKind.InsertAbove => $"✎ {Node}{name} insert {ToNode}",
            RewriteKind.Replace => $"✎ {Node}{name} replace_with {ToNode}",
            RewriteKind.RewriteSubtree => ToNode is not null
                ? $"✎ {Node}{name} rewrite {ToNode}" : $"✎ {Node}{name} rewrite",
            _ => throw ExhaustiveMatch.Failed(Kind)
        };
    }
}
