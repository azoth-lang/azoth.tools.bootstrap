using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// The context established by a rewrite operation.
/// </summary>
internal sealed class RewriteContext
{
    public static readonly RewriteContext Root = new();

    /// <summary>
    /// The index of the attribute being rewritten or <see langword="null"/> for nodes which are not
    /// underneath any rewrite.
    /// </summary>
    public ulong? LowLink { get; }

    /// <summary>
    /// Whether this rewrite context is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    public RewriteContext(ulong lowLink)
    {
        LowLink = lowLink;
    }

    private RewriteContext() { }

    public void MarkInactive()
    {
        if (LowLink is null)
            throw new InvalidOperationException("Cannot mark the root context as inactive.");
        IsActive = false;
    }
}
