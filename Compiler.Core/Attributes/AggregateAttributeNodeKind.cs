using System;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

[Flags]
public enum AggregateAttributeNodeKind
{
    // Kinds
    Neutral = SubtreeMayContribute,
    Contributor = MayContribute | SubtreeMayContribute,
    Attribute = HasAttribute,
    AttributeWithContributor = HasAttribute | MayContribute,

    // Flags
    HasAttribute = 1 << 0,
    MayContribute = 1 << 1,
    SubtreeMayContribute = 1 << 2,
}

public static class AggregateAttributeNodeKindExtensions
{
    [Inline(export: true)]
    public static bool HasAttribute(this AggregateAttributeNodeKind kind)
        => (kind & AggregateAttributeNodeKind.HasAttribute) != 0;

    [Inline(export: true)]
    public static bool MayContribute(this AggregateAttributeNodeKind kind)
        => (kind & AggregateAttributeNodeKind.MayContribute) != 0;

    [Inline(export: true)]
    public static bool SubtreeMayContribute(this AggregateAttributeNodeKind kind)
        => (kind & AggregateAttributeNodeKind.SubtreeMayContribute) != 0;
}
