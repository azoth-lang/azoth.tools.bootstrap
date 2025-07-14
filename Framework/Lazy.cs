using System;
using System.Runtime.CompilerServices;
using System.Threading;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// This class exists to avoid the delegate allocations that are required when using
/// <see cref="LazyInitializer"/>. By using static lambda arguments with this class, the delegates
/// will be statically cached.
/// </summary>
/// <remarks>Code based on <c>NonCapturingLazyInitializer</c> from EF core.</remarks>
public static class Lazy
{
    /// <summary>
    /// A shorthand for creating instances of <see cref="Lazy{T}"/> without specifying the type.
    /// </summary>
    [Inline(export: true)]
    public static Lazy<T> Create<T>(Func<T> valueFactory) => new(valueFactory);

    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    /// <remarks>If called concurrently, the <paramref name="valueFactory"/> will only be called
    /// once. To accomplish this, <paramref name="target"/> is temporarily set to an invalid value.
    /// One must never use that value. Always access via <see cref="InitializeOnce{TValue}"/>.</remarks>
    public static TValue InitializeOnce<TValue>(ref TValue? target, Func<TValue> valueFactory)
        where TValue : class
    {
        // Fast path
        var tmp = Volatile.Read(ref target);
        if (tmp is not null && !ReferenceEquals(tmp, InProgressValue)) return tmp;

        tmp = Interlocked.CompareExchange(ref target, Unsafe.As<TValue>(InProgressValue), null);
        if (tmp is not null)
        {
            // Another thread is initializing, wait for it to complete
            SpinWait sw = new SpinWait();
            do
            {
                sw.SpinOnce();
            } while (ReferenceEquals(tmp = target, InProgressValue));
        }
        else
            // We've acquired the lock to do the initialization
            Volatile.Write(ref target, tmp = valueFactory());

        return tmp;
    }

    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    /// <remarks>If called concurrently, the <paramref name="valueFactory"/> will only be called
    /// once. To accomplish this, <paramref name="target"/> is temporarily set to an invalid value.
    /// One must never use that value. Always access via <see cref="InitializeOnce{TValue}"/>.</remarks>
    public static TValue InitializeOnce<TParam, TValue>(ref TValue? target, TParam param, Func<TParam, TValue> valueFactory)
        where TValue : class
    {
        // Fast path
        var tmp = Volatile.Read(ref target);
        if (tmp is not null && !ReferenceEquals(tmp, InProgressValue)) return tmp;

        tmp = Interlocked.CompareExchange(ref target, Unsafe.As<TValue>(InProgressValue), null);
        if (tmp is not null)
        {
            // Another thread is initializing, wait for it to complete
            SpinWait sw = new SpinWait();
            do
            {
                sw.SpinOnce();
            } while (ReferenceEquals(tmp = target, InProgressValue));
        }
        else
            // We've acquired the lock to do the initialization
            Volatile.Write(ref target, tmp = valueFactory(param));

        return tmp;
    }


    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    /// <remarks>If called concurrently, the <paramref name="valueFactory"/> could be called
    /// multiple times but only one value will be used.</remarks>
    public static TValue Initialize<TValue>(ref TValue? target, Func<TValue> valueFactory)
        where TValue : class
    {
        var tmp = Volatile.Read(ref target);
        if (tmp != null) return tmp;

        Interlocked.CompareExchange(ref target, valueFactory(), null);

        return target;
    }

    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    public static TValue Initialize<TParam, TValue>(
       ref TValue? target,
       TParam param,
       Func<TParam, TValue> valueFactory)
       where TValue : class
    {
        var tmp = Volatile.Read(ref target);
        if (tmp is not null) return tmp;

        Interlocked.CompareExchange(ref target, valueFactory(param), null);

        return target;
    }

    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    public static TValue Initialize<TParam1, TParam2, TValue>(
        ref TValue? target,
        TParam1 param1,
        TParam2 param2,
        Func<TParam1, TParam2, TValue> valueFactory)
        where TValue : class
    {
        var tmp = Volatile.Read(ref target);
        if (tmp is not null) return tmp;

        Interlocked.CompareExchange(ref target, valueFactory(param1, param2), null);

        return target;
    }

    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    public static TValue? InitializeNullable<TParam1, TParam2, TValue>(
        ref TValue? target,
        TParam1 param1,
        TParam2 param2,
        Func<TParam1, TParam2, TValue?> valueFactory)
        where TValue : class
    {
        var tmp = Volatile.Read(ref target);
        if (ReferenceEquals(tmp, NullValue)) return null;
        if (tmp is not null) return tmp;

        Interlocked.CompareExchange(ref target, valueFactory(param1, param2) ?? Unsafe.As<TValue>(NullValue), null);

        tmp = target;
        return ReferenceEquals(tmp, NullValue) ? null : tmp;
    }

    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    public static TValue Initialize<TParam1, TParam2, TParam3, TValue>(
        ref TValue? target,
        TParam1 param1,
        TParam2 param2,
        TParam3 param3,
        Func<TParam1, TParam2, TParam3, TValue> valueFactory)
        where TValue : class
    {
        var tmp = Volatile.Read(ref target);
        if (tmp is not null) return tmp;

        Interlocked.CompareExchange(ref target, valueFactory(param1, param2, param3), null);

        return target;
    }

    /// <summary>
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
    public static TValue Initialize<TParam, TValue>(
        ref TValue target,
        ref bool initialized,
        TParam param,
        Func<TParam, TValue> valueFactory)
        where TValue : class?
    {
        var alreadyInitialized = Volatile.Read(ref initialized);
        if (alreadyInitialized) return target;

        var oldValue = target;
        var newValue = valueFactory(param);
        // Try to avoid changing to a new value by using atomic compare exchange
        Interlocked.CompareExchange(ref target!, newValue, oldValue);
        Volatile.Write(ref initialized, true);

        return target;
    }

    public static TValue Initialize<TValue>(
        ref TValue? target,
        TValue value)
        where TValue : class
    {
        var tmp = Volatile.Read(ref target);
        if (tmp != null) return tmp;

        Interlocked.CompareExchange(ref target, value, null);

        return target;
    }

    private static readonly object InProgressValue = new();
    private static readonly object NullValue = new();
}
