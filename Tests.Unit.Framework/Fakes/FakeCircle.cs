using System;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework.Fakes;

internal sealed class FakeCircle : FakeShape
{
    public FakeCircle(int radius)
    {
        Radius = radius;
    }

    public int Radius { get; }

    public override bool Equals(FakeShape? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FakeSquare that && Radius == that.SideLength;
    }

    public override int GetHashCode() => HashCode.Combine(Radius);
}
