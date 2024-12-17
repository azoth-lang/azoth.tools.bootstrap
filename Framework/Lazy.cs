using System;
using System.Threading;

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
    /// Initializes a target reference type with a specified function if it has not already been
    /// initialized. Important use a <b>static lambda</b> as the parameter for good performance.
    /// </summary>
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
        if (tmp != null)
            return tmp;

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
        if (tmp != null) return tmp;

        Interlocked.CompareExchange(ref target, valueFactory(param1, param2), null);

        return target;
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
        if (tmp != null) return tmp;

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
        if (alreadyInitialized) return Volatile.Read(ref target);

        Volatile.Write(ref target, valueFactory(param));
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
}
