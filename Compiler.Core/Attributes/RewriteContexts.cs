using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// A collection of rewrite contexts for nodes.
/// </summary>
/// <remarks><para>It might be possible to use <see cref="ImmutableDictionary{TKey,TValue}"/> to
/// create a nested cache at each layer so that as contexts were marked inactive one could roll back
/// to a cache without the child rewrite context. However, that would be a large refactoring and may
/// not improve performance.</para></remarks>
[DebuggerStepThrough]
internal sealed class RewriteContexts
{
    /// <summary>
    /// A dictionary of nodes to their rewrite contexts.
    /// </summary>
    /// <remarks>If an entry in the dictionary refers to an inactive context, it does not matter for
    /// determining the low link. However, it is still an indication that the node is not in the
    /// final tree.</remarks>
    private readonly Dictionary<ITreeNode, RewriteContext> contexts
        = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// A cache of rewrite contexts for nodes.
    /// </summary>
    /// <remarks>If a cached context is active, it is correct. If it is inactive, then the cache
    /// could be stale and there could be a parent context that is active. Entries are not removed
    /// from the cache except by clearing the whole cache to avoid a situation where a parent node
    /// is removed from the cache without child nodes being removed and thereby causing incorrect
    /// cache clearing.</remarks>
    private readonly Dictionary<ITreeNode, RewriteContext> contextCache
        = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Add a new rewrite context for the specified node.
    /// </summary>
    /// <returns>The newly added <see cref="RewriteContext"/>.</returns>
    public RewriteContext NewRewrite(IChildTreeNode node, ulong index)
    {
        Requires.That(!node.InFinalTree, nameof(node), "Cannot be in final tree already.");
        var context = ActiveContextFor(node.PeekParent()!);
        Requires.That(context is null || context.AttributeIndex < index, nameof(index), "Must be within the ancestor rewrite.");
        InvalidateCacheFor(node);
        return contexts[node] = new(context, index);
    }

    /// <summary>
    /// The <paramref name="node"/> has been rewritten to <paramref name="rewrittenNode"/>.
    /// Associate the new node with the rewrite context of the previous node.
    /// </summary>
    public void AddRewriteToContext(IChildTreeNode node, IChildTreeNode? rewrittenNode, ulong attributeIndex)
    {
        if (rewrittenNode is null) return;

        // Node should have its own rewrite context and not need to inherit one
        if (!contexts.TryGetValue(node, out var ctx))
            throw new InvalidOperationException("Existing node should have a rewrite context.");
#if DEBUG
        if (node.InFinalTree || !ctx.IsActive || ctx.AttributeIndex != attributeIndex)
            throw new InvalidOperationException("Existing node should not be in the final tree and context should be active for this attribute.");
#endif
        InvalidateCacheFor(node);
        contexts[rewrittenNode] = ctx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong? LowLinkFor(IChildTreeNode node)
        => ActiveContextFor(node)?.LowLink;

    /// <summary>
    /// Invalidate any cached values for this node and its children.
    /// </summary>
    [Inline]
    private void InvalidateCacheFor(IChildTreeNode node)
    {
        // If a context has been cached for this node then there is a risk that outdated contexts
        // have been cached for child nodes. Since there is no efficient way to invalidate just the
        // caches for child nodes, clear the whole cache.
        if (contextCache.ContainsKey(node))
            contextCache.Clear();
    }

    /// <summary>
    /// The active context for this tree node or <see langword="null"/> for nodes without an active
    /// context.
    /// </summary>
    private RewriteContext? ActiveContextFor(ITreeNode node)
    {
        var ctx = ContextFor(node, contexts, contextCache);
        return ctx?.IsActive == true ? ctx : null;

        // Get the (possibly inactive) context for the given node and update contextCache to cache
        // contexts for more efficient lookup in the future.
        static RewriteContext? ContextFor(
            ITreeNode node,
            Dictionary<ITreeNode, RewriteContext> contexts,
            Dictionary<ITreeNode, RewriteContext> contextCache)
        {
            if (node.InFinalTree)
                return null;

            // Check if the node is cached
            if (contextCache.TryGetValue(node, out var cachedContext) && cachedContext.IsActive)
                return cachedContext;

            // If the node has a cached inactive context, it will be removed or replaced with a new
            // context below if needed.

            // Not cached, or cached inactive, check if associated with a new active context.
            if (contexts.TryGetValue(node, out var context) && context.IsActive)
            {
                contextCache[node] = context;
                return context;
            }

            // Recursively search for the context of the parent node.
            var parent = node.PeekParent();
            var parentContext = parent is not null
                ? ContextFor(parent, contexts, contextCache)
                // This node is the root of the tree and as such is not part of a rewrite context.
                : throw new InvalidOperationException($"Root node should already be {nameof(ITreeNode.InFinalTree)}");

            if (parentContext?.IsActive == true)
            {
                // This node did not have an active cached context. Update the cache so that future
                // lookups can find the active context faster.
                return contextCache[node] = parentContext;
            }

            // Inherit parent context if no context for node.
            context ??= parentContext;

            if (context is null)
                // This node is above any active or inactive rewrite context, mark it as final.
                node.MarkInFinalTree();

            // In the else case, there is no reason to update the cache. Either the context is
            // unchanged, or the parent is inactive and so would still require searching up the tree
            // to look for an active context. Indeed, by not caching we may avoid needing to clear
            // the cache later.

            return context;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        contextCache.Clear();
        contexts.Clear();
    }
}
