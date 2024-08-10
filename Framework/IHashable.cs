namespace Azoth.Tools.Bootstrap.Framework;

public interface IHashable<in TSelf> : IEqualTo<TSelf>
    where TSelf : IHashable<TSelf>?
{
    int GetHashCode();
}
