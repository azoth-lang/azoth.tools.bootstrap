namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

public readonly struct DowncastingPromise<T>
{
    private readonly IPromise<T> promise;

    internal DowncastingPromise(IPromise<T> promise)
    {
        this.promise = promise;
    }

    /// <summary>
    /// Downcast this promise to another kind of promise if possible.
    /// </summary>
    public IPromise<TSub>? As<TSub>()
        where TSub : T
    {
        if (promise is IPromise<TSub> convertedPromise) return convertedPromise;
        if (promise is { IsFulfilled: true, Result: TSub convertedValue })
            return new Promise<TSub>(convertedValue);
        return null;
    }
}
