using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Framework;

public static class EnumerableExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<T> Yield<T>(this T value)
    {
        yield return value;
    }

    [DebuggerStepThrough]
    public static IEnumerable<T> YieldValue<T>(this T? value)
        where T : class
    {
        if (value is not null) yield return value;
    }

    [DebuggerStepThrough]
    public static IEnumerable<T> YieldValue<T>(this T? value)
        where T : struct
    {
        if (value is not null) yield return value.Value;
    }

    [DebuggerStepThrough]
    public static IFixedList<T> ToFixedList<T>(this IEnumerable<T> values)
        => values as IFixedList<T> ?? FixedList.Create(values);

    [DebuggerStepThrough]
    public static FixedSet<T> ToFixedSet<T>(this IEnumerable<T> values)
        => values as FixedSet<T> ?? new(values);

    [DebuggerStepThrough]
    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        Func<TFirst, TSecond, TResult> resultSelector)
        => first.SelectMany(_ => second, resultSelector);

    [DebuggerStepThrough]
    public static IEnumerable<(TFirst, TSecond)> CrossJoin<TFirst, TSecond>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second)
        => first.SelectMany(_ => second, (f, s) => (f, s));

    [DebuggerStepThrough]
    public static IEnumerable<(T Value, int Index)> Enumerate<T>(this IEnumerable<T> source)
        => source.Select((v, i) => (v, i));

    [DebuggerStepThrough]
    public static Queue<T> ToQueue<T>(this IEnumerable<T> source) => new(source);

    [DebuggerStepThrough]
    public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source)
        => source.SelectMany(items => items);

    [DebuggerStepThrough]
    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T value)
        => source.Except(value.Yield());

    /// <summary>
    /// Performs an implicit cast. This is useful when C# is having trouble getting the correct type.
    /// </summary>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> SafeCast<T>(this IEnumerable<T> source)
        // When the source implements multiple IEnumerable<T> and the next
        // Linq function takes IEnumerable (not generic) this shim
        // is needed to force the call to the correct GetEnumerator().
        // A Linq function that takes IEnumerable is OfType<T>()
        => new ImplicitCastEnumerable<T>(source);

    private class ImplicitCastEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> source;

        public ImplicitCastEnumerable(IEnumerable<T> source)
        {
            this.source = source;
        }

        public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => source.GetEnumerator();
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyTrue(this IEnumerable<bool> values) => values.Any(v => v);

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values)
        where T : class
        => values.Where(v => v is not null)!;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values)
        where T : struct
        => values.Where(v => v is not null).Select(v => v!.Value);

    public static IEnumerable<(T1, T2)> Where<T1, T2>(this IEnumerable<(T1, T2)> source, Func<T1, T2, bool> predicate)
        => source.Where(tuple => predicate(tuple.Item1, tuple.Item2));

    public static IEnumerable<TResult> Select<T1, T2, TResult>(this IEnumerable<(T1, T2)> source, Func<T1, T2, TResult> selector)
        => source.Select(tuple => selector(tuple.Item1, tuple.Item2));
}
