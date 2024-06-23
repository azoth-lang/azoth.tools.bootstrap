using System.Runtime.CompilerServices;
using System.Threading;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public struct Circular<T> : ICyclic<T>
    where T : class?
{
    private object? rawValue;

    public readonly bool IsInitialized => !ReferenceEquals(rawValue, UnsetAttribute.Instance);

    public readonly T UnsafeValue => Unsafe.As<T>(rawValue)!;

    /// <remarks>Circular attribute values are never final. They must be evaluated to see if they </remarks>
    public readonly bool IsFinal => false;

    public Circular(T value)
    {
        rawValue = value;
    }

    private Circular(object? rawValue)
    {
        this.rawValue = rawValue;
    }

    /// <summary>
    /// Initializes the value if it is already initialized.
    /// </summary>
    /// <returns>If the value was changed before it could be initialized, it may not be
    /// <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICyclic<T>.Initialize(T value)
        => Interlocked.CompareExchange(ref rawValue, value, UnsetAttribute.Instance);

    /// <summary>
    /// Atomically compares the current value with <paramref name="comparand"/> and, if they are equal,
    /// exchanges the value with <paramref name="value"/>.
    /// </summary>
    /// <returns>The original value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    T ICyclic<T>.CompareExchange(T value, T comparand)
        => Unsafe.As<T>(Interlocked.CompareExchange(ref rawValue, value, comparand))!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Circular<T>(Circular _) => new(UnsetAttribute.Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Circular<T>(T value) => new(value);
}


public readonly struct Circular
{
    /// <summary>
    /// A value that must be assigned to a <see cref="Circular{T}"/> so it will start uninitialized.
    /// </summary>
    public static Circular Unset => default;
}
