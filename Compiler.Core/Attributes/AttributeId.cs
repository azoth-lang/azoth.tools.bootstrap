using System;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// Uniquely identifies an attribute.
/// </summary>
/// <remarks>This struct assumes that the attribute name will be reference equal for the same
/// attribute. That will typically happen automatically given that attribute names are generated
/// through nameof or the <see cref="CallerMemberNameAttribute"/>. Otherwise, callers will need
/// to use string interning.</remarks>
internal readonly struct AttributeId(ITreeNode node, string attributeName) : IEquatable<AttributeId>
{
    public readonly ITreeNode Node = node;
    private readonly string attributeName = attributeName;

    #region Equality
    public bool Equals(AttributeId other)
        => ReferenceEquals(Node, other.Node)
           && ReferenceEquals(attributeName, other.attributeName);

    public override bool Equals(object? obj) => obj is AttributeId other && Equals(other);

    public override int GetHashCode()
        // Use RuntimeHelpers.GetHashCode to avoid calling GetHashCode on the node and attributeName
        // since they are compared by reference.
        => HashCode.Combine(RuntimeHelpers.GetHashCode(Node), RuntimeHelpers.GetHashCode(attributeName));

    public static bool operator ==(AttributeId left, AttributeId right) => left.Equals(right);

    public static bool operator !=(AttributeId left, AttributeId right) => !left.Equals(right);
    #endregion

    public string ToTypeString() => $"{Node.GetType().GetFriendlyName()}.{attributeName}";

    public override string ToString() => $"{RuntimeHelpers.GetHashCode(Node)}.{attributeName}";
}
