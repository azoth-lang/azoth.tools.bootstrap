using System.Collections.Generic;

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
    public RewriteContext? NewRewrite(ITreeNode node, ulong index)
    {
        var currentContext = ContextFor(node);
        if (currentContext.LowLink is not null)
            return null;
        var newContext = new RewriteContext(index);
        contextNodes[node] = newContext;
        return newContext;
    }

    public void AddRewriteToContext(ITreeNode node, ITreeNode? rewrittenNode)
    {
        if (rewrittenNode is null)
            return;
        contextNodes[node] = ContextFor(node);
    }

    public RewriteContext ContextFor(ITreeNode node)
    {
        if (contextNodes.TryGetValue(node, out var ctx) && ctx.IsActive)
            return ctx;

        // If the node is associated with an inactive context, it doesn't need to be removed because
        // it will be replaced with a new context below.

        var parent = node.PeekParent();
        ctx = parent is null
            // This node is the root of the tree and as such is not part of a rewrite context. Track
            // this by associating it with the root context.
            ? RewriteContext.Root
            // Recursively search for the context of the parent node and cache the result.
            : ContextFor(parent);

        contextNodes[node] = ctx;
        return ctx;
    }
}
