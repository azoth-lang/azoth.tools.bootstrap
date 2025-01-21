using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class RewriteRuleSyntax
{
    public SymbolSyntax Node { get; }
    public RewriteKind Kind { get; }
    public SymbolSyntax? ToNode { get; }
    public string? Name { get; }

    public RewriteRuleSyntax(SymbolSyntax node, RewriteKind kind, SymbolSyntax? toNode, string? name)
    {
        Node = node;
        Name = name;
        Kind = kind;
        ToNode = toNode;
    }

    public override string ToString()
        => Kind switch
        {
            RewriteKind.InsertAbove => $"✎ {Node} insert {ToNode}",
            RewriteKind.Replace => $"✎ {Node} replace_with {ToNode}",
            RewriteKind.Subtree => Name is not null ? $"✎ {Node} {Name}" : $"✎ {Node} rewrite",
            _ => throw ExhaustiveMatch.Failed(Kind)
        };
}
