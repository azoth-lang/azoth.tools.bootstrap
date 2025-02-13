using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// The context established by a rewrite operation.
/// </summary>
[DebuggerStepThrough]
internal sealed class RewriteContext
{
    /// <summary>
    /// The index of the attribute being rewritten.
    /// </summary>
    public ulong LowLink { get; }

    /// <summary>
    /// Whether this rewrite context is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    public RewriteContext(ulong lowLink)
    {
        LowLink = lowLink;
    }

    public void MarkInactive() => IsActive = false;
}
