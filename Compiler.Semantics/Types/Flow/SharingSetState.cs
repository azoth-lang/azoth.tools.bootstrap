using System;
using Azoth.Tools.Bootstrap.Framework.Collections;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

internal readonly struct SharingSetState : IMergeable<SharingSetState>, IEquatable<SharingSetState>
{
    public bool IsLent { get; }

    public SharingSetState(bool isLent)
    {
        IsLent = isLent;
    }

    public SharingSetState Merge(SharingSetState other) => new(IsLent || other.IsLent);

    #region Equality
    public bool Equals(SharingSetState other)
        => IsLent == other.IsLent;

    public override bool Equals(object? obj) => obj is SharingSetState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsLent);
    #endregion
}
