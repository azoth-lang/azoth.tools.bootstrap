namespace Azoth.Tools.Bootstrap.Framework;

public interface IPartiallyEqualTo<in TSelf>
    where TSelf : IPartiallyEqualTo<TSelf>?
{
    bool? EqualTo(TSelf other);
}
