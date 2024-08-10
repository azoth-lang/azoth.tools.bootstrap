namespace Azoth.Tools.Bootstrap.Framework;

public interface IEqualTo<in TSelf> : IPartiallyEqualTo<TSelf>
    where TSelf : IEqualTo<TSelf>?
{
    bool CanEqual(TSelf other);

    new bool EqualTo(TSelf other);

    bool? IPartiallyEqualTo<TSelf>.EqualTo(TSelf other) => EqualTo(other);
}
