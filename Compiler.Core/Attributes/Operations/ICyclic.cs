namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes.Operations;

public interface ICyclic<T>
    where T : class?
{
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
    /// Whether the value is final and cannot be changed.
    /// </summary>
    /// <remarks>If it is final, it should be immediately cached.</remarks>
    bool IsFinal { get; }

    internal void Initialize(T value);

    internal T CompareExchange(T value, T comparand);
}
