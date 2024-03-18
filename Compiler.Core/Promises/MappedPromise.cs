using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

/// <summary>
/// A promise derived from another promise by transforming the value.
/// </summary>
internal sealed class MappedPromise<T, S> : IPromise<S>
{
    private readonly IPromise<T> promise;
    private readonly Func<T, S> selector;

    public MappedPromise(IPromise<T> promise, Func<T, S> selector)
    {
        this.promise = promise;
        this.selector = selector;
    }

    public bool IsFulfilled => promise.IsFulfilled;

    public S Result => selector(promise.Result);

    public override string ToString()
        => IsFulfilled ? Result?.ToString() ?? Promise.NullString : Promise.PendingString;
}
