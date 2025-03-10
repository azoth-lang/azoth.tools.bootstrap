using System.Diagnostics;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[DebuggerStepThrough]
[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
internal readonly struct AttributeCachedRef
{
    public AttributeId Attribute { get; }
    private readonly InteriorRef<bool> cached;
    public ref bool Cached => ref cached.Value;

    public AttributeCachedRef(in AttributeId attribute, InteriorRef<bool> cached)
    {
        Attribute = attribute;
        this.cached = cached;
    }

    public override string ToString() => $"{Attribute}.Cached: {Cached}";
}
