using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

public sealed class ImmutableDisjointHashSets<TItem, TItemData, TSetData> : IImmutableDisjointSets<TItem, TItemData, TSetData>
    where TItem : notnull
    where TSetData : IMergeable<TSetData>, IEquatable<TSetData>
{
    public static ImmutableDisjointHashSets<TItem, TItemData, TSetData> Empty { get; } = new();

    private readonly ImmutableDictionary<TItem, ItemData<TItemData>> items;
    private readonly ImmutableArray<ImmutableDisjointHashSet<TItem, TSetData>?> sets;
    private readonly int setCount;

    public int Count => items.Count;

    public IEnumerable<TItem> Items => items.Keys;
    IEnumerable<TItem> IReadOnlyDictionary<TItem, TItemData>.Keys => items.Keys;
    IEnumerable<TItemData> IReadOnlyDictionary<TItem, TItemData>.Values => items.Values.Select(d => d.Data);

    public IImmutableDisjointSets<TItem, TItemData, TSetData>.ISetCollection Sets { get; }

    internal ImmutableDisjointHashSets(
        ImmutableDictionary<TItem, ItemData<TItemData>> items,
        ImmutableArray<ImmutableDisjointHashSet<TItem, TSetData>?> sets,
        int setCount)
    {
        this.items = items;
        this.sets = sets;
        this.setCount = setCount;
        Sets = new SetCollection(this);
    }

    private ImmutableDisjointHashSets()
        : this(ImmutableDictionary<TItem, ItemData<TItemData>>.Empty, ImmutableArray<ImmutableDisjointHashSet<TItem, TSetData>?>.Empty, 0)
    {
    }

    public TItemData this[TItem item] => items[item].Data;

    public bool Contains(TItem item) => items.ContainsKey(item);
    public bool TryGetValue(TItem key, [MaybeNullWhen(false)] out TItemData value)
    {
        if (items.TryGetValue(key, out var data))
        {
            value = data.Data;
            return true;
        }

        value = default;
        return false;
    }

    public IEnumerator<KeyValuePair<TItem, TItemData>> GetEnumerator()
        // ReSharper disable once NotDisposedResourceIsReturned
        => items.Select(p => KeyValuePair.Create(p.Key, p.Value.Data)).GetEnumerator();

    public IImmutableDisjointSets<TItem, TItemData, TSetData>.IBuilder ToBuilder()
        => new ImmutableDisjointHashSetsBuilder<TItem, TItemData, TSetData>(items, sets, null);

    public IImmutableDisjointSets<TItem, TItemData, TSetData>.IBuilder ToBuilder(
        Action<IImmutableDisjointSets<TItem, TItemData, TSetData>.IBuilder, TSetData> setRemoved)
        => new ImmutableDisjointHashSetsBuilder<TItem, TItemData, TSetData>(items, sets, setRemoved);

    private sealed class SetCollection(ImmutableDisjointHashSets<TItem, TItemData, TSetData> disjointSets) : IImmutableDisjointSets<TItem, TItemData, TSetData>.ISetCollection
    {
        public int Count => disjointSets.setCount;

        public IImmutableDisjointSet<TItem, TSetData> this[TItem item]
            => disjointSets.sets[disjointSets.items[item].SetIndex]!;

        public IImmutableDisjointSet<TItem, TSetData>? TrySetFor(TItem item)
            => disjointSets.items.TryGetValue(item, out var data)
                ? disjointSets.sets[data.SetIndex] : null;

        public IEnumerator<IImmutableDisjointSet<TItem, TSetData>> GetEnumerator()
            // ReSharper disable once NotDisposedResourceIsReturned
            => disjointSets.sets.WhereNotNull().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
