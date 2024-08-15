namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

internal sealed class FlattenedPromise<T> : IPromise<T>
{
    private readonly IPromise<IPromise<T>> promise;

    public FlattenedPromise(IPromise<IPromise<T>> promise)
    {
        this.promise = promise;
    }

    public bool IsFulfilled => promise.IsFulfilled && promise.Result.IsFulfilled;

    public T Result => promise.Result.Result;

    public override string ToString() => promise.ToString();
}
