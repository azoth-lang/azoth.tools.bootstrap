using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public sealed class ReferenceOperations<T> : IAttributeOperations<T, Void>
    where T : class?
{
    private ReferenceOperations() { }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Read(in T? location, ref Void _) => location;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T WriteFinal(ref T? location, T value, ref Void _, ref bool cached)
    {
        // Check cached again since it could have been set while computing the value
        if (cached)
            return location!;
        location = value;
        Volatile.Write(ref cached, true);
        return value;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CompareExchange(
        ref T? location,
        T value,
        T? comparand,
        IEqualityComparer<T> comparer,
        ref Void _,
        out T? previous)
    {
        previous = Interlocked.CompareExchange(ref location, value, comparand);
        return ReferenceEquals(previous, comparand);
    }
}
