namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

public interface IPromise<out T>
{
    bool IsFulfilled { get; }
    T Result { get; }
}
