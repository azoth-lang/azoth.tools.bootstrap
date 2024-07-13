using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

internal class ImmutableDisjointHashSetsBuilder<TItem, TItemData, TSetData>
    : IImmutableDisjointSets<TItem, TItemData, TSetData>.IBuilder
    where TItem : notnull
    where TSetData : IMergeable<TSetData>
{
    private readonly ImmutableDictionary<TItem, ItemData<TItemData>>.Builder items;
    private readonly List<IImmutableDisjointSetBuilder<TItem, TSetData>?> sets;
    private int setCount;
    private PriorityQueue<int>? emptySets;

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

    public void AddToSet(TItem item, int setIndex) => throw new System.NotImplementedException();

    public int Add(TItem item, TSetData data) => throw new System.NotImplementedException();

    public IImmutableDisjointSets<TItem, TItemData, TSetData> ToImmutable()
    {
        TrimSets();
        var immutableSets = ImmutableArray.CreateBuilder<ImmutableDisjointHashSet<TItem, TSetData>?>(sets.Count);

        foreach (var set in sets)
            immutableSets.Add(set?.ToImmutable());
        return new ImmutableDisjointHashSets<TItem, TItemData, TSetData>(
                       items.ToImmutable(), immutableSets.ToImmutable(), setCount);
    }

    private void TrimSets()
    {
        while (sets.Count > 0 && sets[^1] is null)
            sets.RemoveAt(sets.Count - 1);
    }
}
