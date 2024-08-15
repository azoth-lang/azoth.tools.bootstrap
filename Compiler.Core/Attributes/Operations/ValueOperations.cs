using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public sealed class ValueOperations<T> : IAttributeOperations<T, AttributeLock>
{
    private ValueOperations() { }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Read(in T? location, ref AttributeLock syncLock)
        => syncLock.Read(in location)!;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T WriteFinal(ref T? location, T value, ref AttributeLock syncLock, ref bool cached)
    {
        syncLock.WriteFinal(ref location, value, ref cached);
        return value;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CompareExchange(
        ref T? location,
        T value,
        T? comparand,
        IEqualityComparer<T> comparer,
        ref AttributeLock syncLock,
        out T? previous)
        => syncLock.CompareExchange(ref location!, value, comparand!, comparer, out previous);
}
