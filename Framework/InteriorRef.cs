using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Framework;

[DebuggerStepThrough]
public readonly struct InteriorRef<T>
    where T : struct
{
    public object Owner { get; }
    public nint Offset { get; }
    public ref T Value
        => ref Unsafe.AddByteOffset(ref Unsafe.As<Fake>(Owner).StartOfObject, Offset);

    /// <summary>
    /// CAUTION: This is a dangerous method that should be used with care. The <paramref name="owner"/>
    /// must be the object that contains the <paramref name="value"/>.
    /// </summary>
    public InteriorRef(in object owner, ref T value)
    {
        Owner = owner;
        ref T startOfObject = ref Unsafe.As<Fake>(Owner).StartOfObject;
        Offset = Unsafe.ByteOffset(ref startOfObject, ref value);
    }

    private class Fake
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public T StartOfObject;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }
}

[DebuggerStepThrough]
public static class InteriorRef
{
    /// <summary>
    /// CAUTION: This is a dangerous method that should be used with care. The <paramref name="owner"/>
    /// must be the object that contains the <paramref name="value"/>.
    /// </summary>
    public static InteriorRef<T> Create<T>(in object owner, ref T value)
        where T : struct
        => new InteriorRef<T>(owner, ref value);
}
