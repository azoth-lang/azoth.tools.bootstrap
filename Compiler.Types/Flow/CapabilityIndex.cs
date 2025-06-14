using System.Diagnostics;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow;

/// <summary>
/// The index of a capability in a type.
/// </summary>
/// <remarks>This is a tree based index where each number is the zero-based index of the type
/// argument at that level. For example, `∅.1.0.2` is the third argument of the first argument of
/// the second argument of the outer type. The index is displayed starting with `∅` since that is
/// the index of the top-level capability for the type. This capability is represented by the index
/// `∅` because there is no list to index at that level and hence no index. It also provides a clear
/// starting point to the index.</remarks>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[StructLayout(LayoutKind.Auto)]
public readonly struct CapabilityIndex : IEquatable<CapabilityIndex>, IComparable<CapabilityIndex>
{
    public static readonly CapabilityIndex TopLevel = new(FixedList.Empty<int>());

    public IFixedList<int> TreeIndex { get; }

    public bool IsTopLevel => TreeIndex.IsEmpty;

    public CapabilityIndex(IEnumerable<int> treeIndex)
    {
        TreeIndex = treeIndex.ToFixedList();
    }

    public CapabilityIndex Append(int index)
        => new(TreeIndex.Append(index));

    #region Equality and Comparison
    public bool Equals(CapabilityIndex other) => TreeIndex.Equals(other.TreeIndex);

    public override bool Equals(object? obj) => obj is CapabilityIndex other && Equals(other);

    public int CompareTo(CapabilityIndex other)
    {
        var cmp = TreeIndex.Count.CompareTo(other.TreeIndex.Count);
        if (cmp != 0) return cmp;
        for (int i = 0; i < TreeIndex.Count; i++)
        {
            cmp = TreeIndex[i].CompareTo(other.TreeIndex[i]);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    public override int GetHashCode() => TreeIndex.GetHashCode();

    public static bool operator ==(CapabilityIndex left, CapabilityIndex right)
        => left.Equals(right);

    public static bool operator !=(CapabilityIndex left, CapabilityIndex right)
        => !left.Equals(right);
    #endregion

    public override string ToString()
        => string.Join('.', TreeIndex.Select(i => i.ToString()).Prepend("∅"));
}
