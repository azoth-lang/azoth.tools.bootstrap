using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

internal class ImmutableDisjointHashSetsBuilder<TItem, TItemData, TSetData>
    : IImmutableDisjointSets<TItem, TItemData, TSetData>.IBuilder
    where TItem : notnull
    where TSetData : IMergeable<TSetData>, IEquatable<TSetData>
{
    private readonly ImmutableDictionary<TItem, ItemData<TItemData>>.Builder items;
    private readonly List<IImmutableDisjointSetBuilder<TItem, TSetData>?> sets;
    private int setCount;
    private PriorityQueue<int>? emptySets;
    public int Count => items.Count;

    public IEnumerable<TItem> Items => items.Keys;
    IEnumerable<TItem> IReadOnlyDictionary<TItem, TItemData>.Keys => items.Keys;
    IEnumerable<TItemData> IReadOnlyDictionary<TItem, TItemData>.Values => items.Values.Select(d => d.Data);

    public ImmutableDisjointHashSetsBuilder(
        ImmutableDictionary<TItem, ItemData<TItemData>> items,
        ImmutableArray<ImmutableDisjointHashSet<TItem, TSetData>?> sets)
    {
        Requires.That(nameof(sets), sets.Length == 0 || sets[^1] is not null, "Must not be empty sets at end");
        this.items = items.ToBuilder();
        this.sets = [.. sets];
        setCount = sets.Length;
        for (int i = 0; i < sets.Length; i++)
            if (sets[i] is null)
            {
                emptySets ??= new();
                emptySets.Enqueue(i);
                setCount -= 1;
            }
    }

    public TItemData this[TItem item]
    {
        get => items[item].Data;
        set
        {
            // TODO it isn't very efficient to have them read and then we must read again
            if (items.TryGetValue(item, out var data))
                items[item] = data with { Data = value };
            else
                throw new ArgumentException("Item not found.", nameof(item));
        }
    }

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

    public int? TrySetFor(TItem item)
        => items.TryGetValue(item, out var data) ? data.SetIndex : null;

    public int Union(TItem item1, TItem item2)
        => Union(items[item1].SetIndex, items[item2].SetIndex);

    public int Union(int setIndex1, int setIndex2)
    {
        if (setIndex1 == setIndex2)
            return setIndex1;
        var set1 = sets[setIndex1] ?? throw new ArgumentException("Must be a valid set index", nameof(setIndex1));
        var set2 = sets[setIndex2] ?? throw new ArgumentException("Must be a valid set index", nameof(setIndex2));

        // Merge the smaller set into the larger set. If same size, prefer lower index.
        var mergeIntoSet1 = set1.Count > set2.Count || (set1.Count == set2.Count && setIndex1 < setIndex2);
        var (mergeIndex, removeIndex) = mergeIntoSet1 ? (setIndex1, setIndex2) : (setIndex2, setIndex1);

        // Update the set index of each item in the smaller set first. Once the sets objects are
        // merged, the list of items may not be available.
        foreach (var item in mergeIntoSet1 ? set2 : set1)
            items[item] = items[item] with { SetIndex = mergeIndex };

        // Actually merge the sets
        sets[mergeIndex] = Merge(set1, set2);

        // Remove the smaller set from the list of sets.
        DropSet(removeIndex);

        return mergeIndex;
    }

    /// <summary>
    /// Drop a set that is no longer needed.
    /// </summary>
    /// <remarks>This does not make any attempt to remove items that are in the set, that should
    /// already be handled before calling this method</remarks>
    private void DropSet(int removeIndex)
    {
        if (removeIndex == sets.Count - 1)
            sets.RemoveAt(removeIndex);
        else
        {
            sets[removeIndex] = null;
            emptySets ??= new();
            emptySets.Enqueue(removeIndex);
        }

        setCount -= 1;
    }

    private static ImmutableDisjointHashSetBuilder<TItem, TSetData> Merge(
        IImmutableDisjointSetBuilder<TItem, TSetData> set1,
        IImmutableDisjointSetBuilder<TItem, TSetData> set2)
        => (set1, set2) switch
        {
            (ImmutableDisjointHashSet<TItem, TSetData> s1, ImmutableDisjointHashSet<TItem, TSetData> s2)
                // Prefer the larger set to be the target of the merge
                => s1.Count >= s2.Count ? s1.ToBuilder().MergeWith(set2) : s2.ToBuilder().MergeWith(s1),
            (ImmutableDisjointHashSetBuilder<TItem, TSetData> s1, ImmutableDisjointHashSet<TItem, TSetData> s2)
                => s1.MergeWith(s2),
            (ImmutableDisjointHashSet<TItem, TSetData> s1, ImmutableDisjointHashSetBuilder<TItem, TSetData> s2)
                => s2.MergeWith(s1),
            (ImmutableDisjointHashSetBuilder<TItem, TSetData> s1, ImmutableDisjointHashSetBuilder<TItem, TSetData> s2)
                // Prefer the larger set to be the target of the merge
                => s1.Count >= s2.Count ? s1.MergeWith(set2) : s2.MergeWith(s1),
            _ => throw new UnreachableException("Invalid set types."),
        };

    public void Remove(TItem item)
    {
        int setIndex = items[item].SetIndex;
        var set = sets[setIndex] ?? throw new UnreachableException("Set of item is not valid.");
        var updatedSet = set.Remove(item);
        if (updatedSet is null)
            DropSet(setIndex);
        else if (!ReferenceEquals(set, updatedSet))
            sets[setIndex] = updatedSet;

        items.Remove(item);
    }

    public int AddSet(TSetData setData, TItem item, TItemData itemData)
    {
        var builder = ImmutableHashSet.CreateBuilder<TItem>();
        builder.Add(item);
        var set = new ImmutableDisjointHashSetBuilder<TItem, TSetData>(setData, builder);

        if (emptySets is not null && emptySets.TryDequeue(out var setIndex))
            sets[setIndex] = set;
        else
        {
            setIndex = sets.Count;
            sets.Add(set);
        }

        items.Add(item, new(itemData, setIndex));
        setCount += 1;
        return setIndex;
    }

    public void AddToSet(int setIndex, TItem item, TItemData itemData)
    {
        var set = sets[setIndex] ?? throw new ArgumentException("Invalid set index.", nameof(setIndex));
        items.Add(item, new(itemData, setIndex));
        var updatedSet = set.Add(item);
        if (!ReferenceEquals(set, updatedSet))
            sets[setIndex] = updatedSet;
    }

    public IEnumerator<KeyValuePair<TItem, TItemData>> GetEnumerator()
        // ReSharper disable once NotDisposedResourceIsReturned
        => items.Select(p => KeyValuePair.Create(p.Key, p.Value.Data)).GetEnumerator();

    public IImmutableDisjointSets<TItem, TItemData, TSetData> ToImmutable()
    {
        var setsLength = MinSetsLength();
        var immutableSets = ImmutableArray.CreateBuilder<ImmutableDisjointHashSet<TItem, TSetData>?>(setsLength);

        for (int i = 0; i < setsLength; i++)
            immutableSets.Add(sets[i]?.ToImmutable());

        return new ImmutableDisjointHashSets<TItem, TItemData, TSetData>(
                       items.ToImmutable(), immutableSets.ToImmutable(), setCount);
    }

    private int MinSetsLength()
    {
        var lastIndex = sets.Count - 1;
        while (lastIndex >= 0 && sets[lastIndex] is null)
            lastIndex -= 1;
        return lastIndex + 1;
    }
}
