using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A collection of rewrite contexts.
/// </summary>
internal sealed class RewriteContexts
{
    /// <summary>
    /// An incomplete dictionary of nodes to their rewrite contexts. This is updated as queries are
    /// to cache the results and improve performance of later queries.
    /// </summary>
    /// <remarks>If an entry in the dictionary refers to an inactive context, it must be ignored and
    /// removed.</remarks>
    private readonly Dictionary<ITreeNode, RewriteContext> contextNodes
        = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Try to add a new rewrite context for the specified node.
    /// </summary>
    /// <returns>The newly added <see cref="RewriteContext"/> or <see langword="null"/> if it is
    /// already under an existing context.</returns>
    public RewriteContext? NewRewrite(IChildTreeNode node, ulong index)
    {
        var currentContext = ContextFor(node.PeekParent()!);
        if (currentContext is not null)
            return null;
        var newContext = new RewriteContext(index);
        contextNodes[node] = newContext;
        return newContext;
    }

    public void AddRewriteToContext(IChildTreeNode node, IChildTreeNode? rewrittenNode)
    {
        if (rewrittenNode is null)
            return;
        contextNodes[rewrittenNode] = ContextFor(node)
            ?? throw new InvalidOperationException("Existing node should be a rewrite and not in the final tree");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong? LowLinkFor(IChildTreeNode node)
        => ContextFor(node)?.LowLink;

    /// <summary>
    /// The context for this tree node or <see langword="null"/> for nodes in the final tree.
    /// </summary>
    private RewriteContext? ContextFor(ITreeNode node)
    {
        if (node.InFinalTree)
            return null;

        if (contextNodes.TryGetValue(node, out var ctx) && ctx.IsActive)
            return ctx;

        // If the node is associated with an inactive context, it doesn't need to be removed because
        // it will be replaced with a new context below.

        var parent = node.PeekParent();
        ctx = parent is not null
            // Recursively search for the context of the parent node and cache the result.
            ? ContextFor(parent)
            // This node is the root of the tree and as such is not part of a rewrite context.
            : throw new InvalidOperationException($"Root node should already be {nameof(ITreeNode.InFinalTree)}");

        if (ctx is null)
        {
            node.MarkInFinalTree();
            contextNodes.Remove(node);
        }
        else
            contextNodes[node] = ctx;
        return ctx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => contextNodes.Clear();
}
