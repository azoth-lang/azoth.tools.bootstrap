using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Tracks the original and modified capability for a reference.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public readonly record struct FlowCapability(ReferenceCapability Original, bool IsWriteRestricted)
{
    public ReferenceCapability Modified { get; init; } = Original;

    public ReferenceCapability Current
    {
        get
        {
            if (IsWriteRestricted) return Modified.WithoutWrite();
            return Modified;
        }
    }

    public static implicit operator FlowCapability(ReferenceCapability capability)
        => new(capability, false);

    public FlowCapability Alias() => this with { Modified = Modified.Alias() };

    public FlowCapability Freeze() => this with { Modified = Modified.Freeze() };

    public FlowCapability RestrictWrite() => this with { IsWriteRestricted = true };

    public FlowCapability RemoveWriteRestriction() => this with { IsWriteRestricted = false };

    public override string ToString()
    {
        var currentCapability = Current;
        if (Original == currentCapability)
            return Original.ToILString();

        var writeRestrict = IsWriteRestricted ? "-/" : "";
        return $"{Original.ToILString()} {writeRestrict}-> {Current.ToILString()}";
    }
}
