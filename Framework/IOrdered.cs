namespace Azoth.Tools.Bootstrap.Framework;

public interface IOrdered<in TSelf> : IEqualTo<TSelf>
    where TSelf : IOrdered<TSelf>?
{
    Ordering CompareTo(TSelf other);
}
