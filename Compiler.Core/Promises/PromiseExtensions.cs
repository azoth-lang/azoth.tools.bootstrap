using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Promises;

public static class PromiseExtensions
{
    /// <summary>
    /// Prepare to downcast the value in this promise to something.
    /// </summary>
    /// <remarks>There are lots of limitations of the C# type system. This allows for extension like
    /// methods that are generic.</remarks>
    public static DowncastingPromise<T> Downcast<T>(this IPromise<T> promise)
        => new DowncastingPromise<T>(promise);

    /// <summary>
    /// Transform the value in a promise producing another promise.
    /// </summary>
    public static IPromise<S> Select<T, S>(this IPromise<T> promise, Func<T, S> selector)
        => new MappedPromise<T, S>(promise, selector);
}
