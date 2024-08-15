namespace Azoth.Tools.Bootstrap.Framework.Collections;

public interface IMergeable<T>
{
    T Merge(T other);
}
