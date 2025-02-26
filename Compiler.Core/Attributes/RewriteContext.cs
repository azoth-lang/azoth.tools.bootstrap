using System;
using System.Collections.Generic;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// The context established by a rewrite operation.
/// </summary>
[DebuggerStepThrough]
internal sealed class RewriteContext
{
    /// <summary>
    /// The rewrite context, if any, above this one in the tree.
    /// </summary>
    public RewriteContext? Context { get; private set; }

    /// <summary>
    /// The index of the child attribute being rewritten.
    /// </summary>
    public ulong AttributeIndex { get; }

    /// <summary>
    /// The low-link for this context. This is the root context of the rewrite if there are nested
    /// contexts.
    /// </summary>
    public ulong LowLink { get; private set; }

    /// <summary>
    /// Whether this rewrite context is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    private List<RewriteContext>? children;

    public RewriteContext(RewriteContext? context, ulong attributeIndex)
    {
        Context = context;
        AttributeIndex = attributeIndex;
        if (Context is { } ctx)
        {
            Requires.That(ctx.IsActive, nameof(context), "Must be active.");
            LowLink = Math.Min(ctx.LowLink, AttributeIndex);
            ctx.AddChild(this);
        }
        else
            LowLink = attributeIndex;
    }

    public void MarkInactive()
    {
        IsActive = false;
        if (children is null) return;
        // Replace child contexts with this node's context (sometimes a descendant rewrite can
        // complete before the ancestor).
        foreach (var child in children)
            child.UpdateContext(Context);
        children.Clear();
    }

    private void UpdateContext(RewriteContext? context)
    {
        Context = context;
        if (Context is not { } ctx)
        {
            LowLink = AttributeIndex;
            return;
        }
        Requires.That(ctx.IsActive, nameof(context), "Must be active.");
        UpdateLowLink();
    }

    [Inline]
    private void AddChild(RewriteContext child)
    {
        children ??= [];
        children.Add(child);
    }

    /// <remarks><see cref="Context"/> must be a non-null active context when calling this
    /// method.</remarks>
    private void UpdateLowLink()
    {
        ulong newLowLink = Math.Min(Context!.LowLink, AttributeIndex);
        if (LowLink == newLowLink) return;
        LowLink = newLowLink;
        if (children is null) return;
        foreach (var child in children)
            child.UpdateLowLink();
    }
}
