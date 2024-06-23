using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

internal readonly struct AttributeCachedRef
{
    public AttributeId Attribute { get; }
    public nint Offset { get; }
    public ref bool Cached
        => ref Unsafe.AddByteOffset(ref Unsafe.As<Fake>(Attribute.Node).StartOfObject, Offset);

    public AttributeCachedRef(AttributeId attribute, ref bool cached)
    {
        Attribute = attribute;
        ref bool startOfObject = ref Unsafe.As<Fake>(Attribute.Node).StartOfObject;
        Offset = Unsafe.ByteOffset(ref startOfObject, ref cached);
    }

    private class Fake
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public bool StartOfObject;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }
}
