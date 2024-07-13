using System.Collections;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

/// <summary>
/// A collection of disjoint sets.
/// </summary>
public interface IImmutableDisjointSets<TItem, TItemData, TSetData>
    : IReadOnlyDictionary<TItem, TItemData>
{
    IEnumerable<TItem> Items { get; }
    ISetCollection Sets { get; }

    bool Contains(TItem item);
    bool IReadOnlyDictionary<TItem, TItemData>.ContainsKey(TItem key) => Contains(key);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IBuilder ToBuilder();

    public interface ISetCollection : IReadOnlyCollection<IImmutableDisjointSet<TItem, TSetData>>
    {
        IImmutableDisjointSet<TItem, TSetData> this[TItem item] { get; }
    }

    public interface IBuilder
    {
        int Union(TItem item1, TItem item2);
        void Remove(TItem item);
        int Add(TItem item, TSetData data);

        IImmutableDisjointSets<TItem, TItemData, TSetData> ToImmutable();
    }
}
