using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

public static class PersistentList
{
    public static PersistentList<T> Create<T>() => PersistentList<T>.Empty;
}

public abstract class PersistentList<T> : IReadOnlyList<T>
{
    public static PersistentList<T> Empty = new EmptyList();
    public abstract int Count { get; }
    public abstract T this[uint index] { get; }
    T IReadOnlyList<T>.this[int index] => this[checked((uint)index)];
    public abstract PersistentList<T> Add(T value);
    public abstract PersistentList<T> Set(uint index, T value);

    public abstract IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class EmptyList : PersistentList<T>
    {
        public override int Count => 0;
        public override T this[uint index] => throw new IndexOutOfRangeException();
        public override PersistentList<T> Add(T value)
            => new NonEmpty<LeafNode>(1, new(value));
        public override PersistentList<T> Set(uint index, T value)
            => throw new IndexOutOfRangeException();

        public override IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();
    }

    private sealed class NonEmpty<TRoot>(int count, TRoot root) : PersistentList<T>
        where TRoot : struct, INode<TRoot>
    {
        private static readonly int Shift = TRoot.Shift;

        public override int Count { get; } = count;
        private readonly TRoot root = root;
        public override T this[uint index] => root[index << Shift];

        public override PersistentList<T> Add(T value)
        {
            var newRoot = root;
            if (newRoot.Add(value))
                return new NonEmpty<TRoot>(Count + 1, newRoot);
            // node was full, upgrade to next level
            var children = new Buffer<TRoot>(2) { [0] = root, [1] = TRoot.Create(value) };
            return new NonEmpty<Node<TRoot>>(Count + 1, new(children));
        }

        public override PersistentList<T> Set(uint index, T value)
        {
            var newRoot = root;
            newRoot.Set(index << Shift, value);
            return new NonEmpty<TRoot>(Count, newRoot);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            // TODO is there a more efficient way to do this?
            for (uint i = 0; i <= Count; i++)
                yield return this[i];
        }
    }

    /// <summary>
    /// The number of bits of the index per level of the tree.
    /// </summary>
    /// <remarks>Branching factor is 2^<see cref="BitsPerLevel"/>.</remarks>
    private const byte BitsPerLevel = 5;
    private const uint MaxNodeSize = 1u << BitsPerLevel;

    private interface INode<TSelf>
        where TSelf : struct, INode<TSelf>
    {
        static abstract TSelf Create(T value);
        static abstract int Shift { get; }

        T this[uint shiftedIndex] { get; }

        /// <returns>Whether adding was successful. It's not successful if the node is full.</returns>
        bool Add(T value);

        /// <summary>
        /// Modify the tree to set the value at the given index.
        /// </summary>
        /// <param name="shiftedIndex">The index of the item left shifted up
        /// <see cref="PersistentList{T}.BitsPerLevel"/> times the number of levels</param>
        /// <param name="value">The new value to set at the given index.</param>
        void Set(uint shiftedIndex, T value);
    }

    private struct Node<TChild>(Buffer<TChild> children) : INode<Node<TChild>>
        where TChild : struct, INode<TChild>
    {
        public static Node<TChild> Create(T value) => new(new(1) { [0] = TChild.Create(value) });

        public static int Shift => TChild.Shift - BitsPerLevel;

        public readonly T this[uint shiftedIndex]
            => children[shiftedIndex >> (32 - BitsPerLevel)][shiftedIndex << BitsPerLevel];

        public bool Add(T value)
        {
            var count = children.Count;
            var last = children[count - 1];
            if (last.Add(value))
            {
                children = children.Copy();
                children[count - 1] = last;
                return true;
            }

            if (count == MaxNodeSize)
                // Can't increase size, add failed
                return false;
            children = children.CopyAndAdd(TChild.Create(value));
            return true;
        }
        public void Set(uint shiftedIndex, T value)
        {
            children = children.Copy();
            children[shiftedIndex >> (32 - BitsPerLevel)].Set(shiftedIndex << BitsPerLevel, value);
        }
    }

    private struct LeafNode(T item) : INode<LeafNode>
    {
        public static LeafNode Create(T value) => new(value);
        public static int Shift => 32;

        public readonly T this[uint _] => item;
        public readonly bool Add(T value) => false;
        public void Set(uint _, T value) => item = value;
    }
}
