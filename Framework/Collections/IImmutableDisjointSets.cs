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

        IImmutableDisjointSet<TItem, TSetData>? TrySetFor(TItem item);
    }

    public interface IBuilder : IReadOnlyDictionary<TItem, TItemData>
    {
        new TItemData this[TItem key] { get; set; }
        TItemData IReadOnlyDictionary<TItem, TItemData>.this[TItem key] => this[key];

        bool Contains(TItem item);
        bool IReadOnlyDictionary<TItem, TItemData>.ContainsKey(TItem key) => Contains(key);

        int? TrySetFor(TItem item);
        int Union(TItem item1, TItem item2);
        int Union(int setIndex1, int setIndex2);
        void Remove(TItem item);
        int AddSet(TSetData setData, TItem item, TItemData itemData);
        void AddToSet(int setIndex, TItem item, TItemData itemData);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IImmutableDisjointSets<TItem, TItemData, TSetData> ToImmutable();
    }
}
