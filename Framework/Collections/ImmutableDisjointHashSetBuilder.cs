using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

internal class ImmutableDisjointHashSetBuilder<TItem, TSetData> : IImmutableDisjointSetBuilder<TItem, TSetData>
    where TItem : notnull
    where TSetData : IMergeable<TSetData>, IEquatable<TSetData>
{
    private readonly ImmutableHashSet<TItem>.Builder values;
    public TSetData Data { get; private set; }
    public int Count => values.Count;

    internal ImmutableDisjointHashSetBuilder(TSetData data, ImmutableHashSet<TItem>.Builder values)
    {
        Data = data;
        this.values = values;
    }

    public ImmutableDisjointHashSetBuilder<TItem, TSetData> MergeWith(IImmutableDisjointSetBuilder<TItem, TSetData> other)
    {
        values.UnionWith(other);
        Data = Data.Merge(other.Data);
        return this;
    }

    public IImmutableDisjointSetBuilder<TItem, TSetData> Add(TItem item)
    {
        values.Add(item);
        return this;
    }

    public IImmutableDisjointSetBuilder<TItem, TSetData>? Remove(TItem item)
    {
        values.Remove(item);
        return values.Count == 0 ? null : this;
    }

    public IImmutableDisjointSetBuilder<TItem, TSetData> Update(Func<TSetData, TSetData> update)
    {
        Data = update(Data);
        return this;
    }

    public IEnumerator<TItem> GetEnumerator()
        // ReSharper disable once NotDisposedResourceIsReturned
        => values.GetEnumerator();

    public ImmutableDisjointHashSet<TItem, TSetData> ToImmutable() => new(Data, values.ToImmutable());
}
