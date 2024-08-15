using System;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework.Fakes;

internal sealed class FakeSquare : FakeShape
{
    public FakeSquare(int sideLength)
    {
        SideLength = sideLength;
    }

    public int SideLength { get; }

    public override bool Equals(FakeShape? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is FakeSquare that
               && SideLength == that.SideLength;
    }

    public override int GetHashCode() => HashCode.Combine(SideLength);
}
