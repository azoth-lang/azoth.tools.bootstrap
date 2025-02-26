using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A collection of rewrite contexts.
/// </summary>
[DebuggerStepThrough]
internal sealed class RewriteContexts
{
    /// <summary>
    /// An incomplete dictionary of nodes to their rewrite contexts. This is updated as queries are
    /// done to cache the results and improve performance of later queries.
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
    public RewriteContext NewRewrite(IChildTreeNode node, ulong index)
    {
        Requires.That(!node.InFinalTree, nameof(node), "Cannot be in final tree already.");
        var context = ContextFor(node.PeekParent()!);
        Requires.That(context is null || context.AttributeIndex < index, nameof(index), "Must be within the ancestor rewrite.");
        var newContext = new RewriteContext(context, index);
        contextNodes[node] = newContext;
        return newContext;
    }

    /// <summary>
    /// The <paramref name="node"/> has been rewritten to <paramref name="rewrittenNode"/>.
    /// Associate the new node with the rewrite context of the previous node.
    /// </summary>
    public void AddRewriteToContext(IChildTreeNode node, IChildTreeNode? rewrittenNode, ulong attributeIndex)
    {
        if (rewrittenNode is null) return;

        // Node should have its own rewrite context and not need to inherit one
        if (!contextNodes.TryGetValue(node, out var ctx))
            throw new InvalidOperationException("Existing node should have a rewrite context.");
#if DEBUG
        if (node.InFinalTree || !ctx.IsActive || ctx.AttributeIndex != attributeIndex)
            throw new InvalidOperationException("Existing node should not be in the final tree and context should be active for this attribute.");
#endif

        contextNodes[rewrittenNode] = ctx;
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

        // If the node is associated with an inactive context, it doesn't need to be removed yet
        // because it will be replaced with a new context or removed below.

        var parent = node.PeekParent();
        ctx = parent is not null
            // Recursively search for the context of the parent node and cache the result.
            ? ContextFor(parent)
            // This node is the root of the tree and as such is not part of a rewrite context.
            : throw new InvalidOperationException($"Root node should already be {nameof(ITreeNode.InFinalTree)}");

        if (ctx is null)
        {
            // This node is above any rewrite context, mark it as final.
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
