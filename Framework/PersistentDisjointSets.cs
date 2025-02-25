using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Azoth.Tools.Bootstrap.Framework;

[StructLayout(LayoutKind.Auto)]
public readonly struct PersistentDisjointSets<T>
{
    public static readonly PersistentDisjointSets<T> Empty = default;

    private readonly PersistentList<Node>? items;

    private PersistentDisjointSets(PersistentList<Node> items)
    {
        this.items = items;
    }

    public uint Count => items?.Count ?? 0;

    public PersistentDisjointSets<T> AddSet()
        => new((items ?? PersistentList<Node>.Empty).Add(Node.CreateRoot(Count)));

    /// <summary>
    /// Find the representative of the set containing the given item.
    /// </summary>
    public uint Find(uint item)
    {
        if (items is null)
            throw new ArgumentOutOfRangeException(nameof(item), "No sets to find item in.");
        uint currentIndex = item;
        ref var currentNode = ref items.Modify(currentIndex);
        while (!currentNode.IsRoot)
        {
            // Grab parent
            var parentIndex = currentNode.ParentOrTail;
            ref var parentNode = ref items.Modify(parentIndex);
            // Path compression (use volatile read/write to avoid reordering)
            Volatile.Write(ref currentNode.ParentOrTail, parentNode.VolatileParent);
            // Move up
            currentIndex = parentIndex;
            currentNode = ref parentNode;
        }
        return currentIndex;
    }

    public PersistentDisjointSets<T> Union(uint item1, uint item2)
    {
        if (items is null)
            throw new ArgumentOutOfRangeException(nameof(item1), "No sets to union.");
        var set1 = Find(item1);
        var set2 = Find(item2);
        if (set1 == set2)
            // Already in the same set
            return this;

        var set1Node = items[set1];
        var set2Node = items[set2];

        var flip = set1Node.Count < set2Node.Count;
        var largerSet = flip ? set2 : set1;
        ref var largerSetNode = ref (flip ? ref set2Node : ref set1Node);
        ref var smallerSetNode = ref (flip ? ref set1Node : ref set2Node);


        largerSetNode = largerSetNode with
        {
            Count = largerSetNode.Count + smallerSetNode.Count,
            ParentOrTail = smallerSetNode.ParentOrTail,
        };
        smallerSetNode = smallerSetNode with
        {
            Count = 0,
            ParentOrTail = largerSet,
        };

        return new(items.Set(set1, set1Node, set2, set2Node));
    }

    [StructLayout(LayoutKind.Auto)]
    private record struct Node
    {
        public static Node CreateRoot(uint item) => new Node(item);

        /// <summary>
        /// The size of this set or zero if this is not a root.
        /// </summary>
        public uint Count { get; init; }

        public readonly bool IsRoot => Count > 0;

        /// <summary>
        /// For root nodes, the tail of the linked list of elements in the set. For non-root nodes,
        /// the parent of this node.
        /// </summary>
        public uint ParentOrTail;

        /// <summary>
        /// The parent of this node or <see cref="uint.MaxValue"/> if this is a root node.
        /// </summary>
        public readonly uint VolatileParent => Count > 0 ? uint.MaxValue : Volatile.Read(in ParentOrTail);

        /// <summary>
        /// The next item in the set, or <see cref="uint.MaxValue"/> if this is the end of the
        /// linked list of elements.
        /// </summary>
        public readonly uint Next;


        private Node(uint item)
        {
            Count = 0;
            ParentOrTail = item;
            Next = uint.MaxValue;
        }
    }
}
