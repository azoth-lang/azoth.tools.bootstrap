using System;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework.Fakes;

/// <summary>
/// A fake class used to test equality of collections.
/// </summary>
internal abstract class FakeShape : IEquatable<FakeShape>
{
    public abstract bool Equals(FakeShape? other);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is FakeShape other && Equals(other);

    public abstract override int GetHashCode();
}
