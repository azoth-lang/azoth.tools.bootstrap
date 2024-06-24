namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public interface ICyclic<T>
    where T : class?
{
    /// <summary>
    /// Whether this is a rewritable attribute.
    /// </summary>
    static abstract bool IsRewritableAttribute { get; }

    /// <summary>
    /// Whether the value is final and cannot be changed.
    /// </summary>
    /// <remarks>If it is final, it should be immediately cached.</remarks>
    static abstract bool IsFinalValue(T value);

    /// <summary>
    /// Whether the value has been initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Get the value of the attribute.
    /// </summary>
    /// <remarks>This property is unsafe if the value has not been initialized. Do not access it
    /// on uninitialized values.</remarks>
    T UnsafeValue { get; }

    /// <summary>
    /// Initializes the value if it isn't already initialized.
    /// </summary>
    /// <returns>If the value was changed before it could be initialized, it may not be
    /// <paramref name="value"/>.</returns>
    internal void Initialize(T value);

    /// <summary>
    /// Atomically compares the current value with <paramref name="comparand"/> and, if they are equal,
    /// exchanges the value with <paramref name="value"/>.
    /// </summary>
    /// <returns>The original value.</returns>
    internal T CompareExchange(T value, T comparand);
}
