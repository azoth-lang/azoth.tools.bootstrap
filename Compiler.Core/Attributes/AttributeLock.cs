using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public struct AttributeLock
{
    private static readonly UIntPtr LockedState = nuint.MaxValue;
    private static readonly UIntPtr MaxVersion = LockedState - 1;

    /// <remarks>When locked, equals <see cref="LockedState"/>. Otherwise, is used as a version that
    /// is incremented each time a value is written. This is used to safely do a compare and
    /// exchange without holding the lock during the comparison.</remarks>
    private nuint version;

    public T Read<T>(in T location)
    {
        var waiter = new SpinWait();
        // Enter the lock
        nuint currentVersion;
        while ((currentVersion = Interlocked.Exchange(ref version, LockedState)) == LockedState)
            waiter.SpinOnce();

        var value = location;

        // Exit the lock
        Volatile.Write(ref version, currentVersion);

        return value;
    }

    public void WriteFinal<T>(ref T location, T value, ref bool cached)
    {
        if (cached)
            return;

        var waiter = new SpinWait();

        // Enter the lock
        nuint currentVersion;
        while ((currentVersion = Interlocked.Exchange(ref version, LockedState)) == LockedState)
            waiter.SpinOnce();

        // Check cached again since it could have been set while waiting for the lock.
        if (!cached)
        {
            // Only write the value if it hasn't been cached yet to avoid issues with readers
            // that read without the lock after it is cached.
            location = value;
            currentVersion = Next(currentVersion);
            // Not certain if this is needed but since cached is read outside the lock, it might be
            Volatile.Write(ref cached, true);
        }

        // No need to use volatile since the lock is held and `cached` was written volatile (or no
        // read or write happened).
        version = currentVersion;
    }

    /// <summary>
    /// Do an atomic compare and exchange using the lock to protect reads and writes while avoiding
    /// holding the lock while calling the external comparer.
    /// </summary>
    public bool CompareExchange<T>(ref T location, T value, T comparand, IEqualityComparer<T> comparer, out T previous)
    {
        var waiter = new SpinWait();
        nuint previousVersion, currentVersion;
        do
        {
            // Enter the lock
            waiter.Reset();
            while ((previousVersion = Interlocked.Exchange(ref version, LockedState)) == LockedState)
                waiter.SpinOnce();

            // Read the value
            previous = location;

            // Exit the lock
            Volatile.Write(ref version, previousVersion);

            if (!comparer.Equals(previous, comparand))
                return false;

            // Enter the lock if version hasn't changed
            waiter.Reset();
            while ((currentVersion = Interlocked.CompareExchange(ref version, LockedState, previousVersion)) == LockedState)
                waiter.SpinOnce();

            // If currentVersion != previousVersion, another write happened between the two locks
            // and also we did not enter the lock.
        } while (currentVersion != previousVersion);

        // Note: lock is held here

        // Write the value
        location = value;

        // Exit the lock
        Volatile.Write(ref version, Next(currentVersion));
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static nuint Next(nuint version) => version == MaxVersion ? 0 : version + 1;
}
