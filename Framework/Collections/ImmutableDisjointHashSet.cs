using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

internal sealed class ImmutableDisjointHashSet<TItem, TSetData>
    : IImmutableDisjointSet<TItem, TSetData>, IImmutableDisjointSetBuilder<TItem, TSetData>
    where TItem : notnull
    where TSetData : IMergeable<TSetData>, IEquatable<TSetData>
{
    private readonly ImmutableHashSet<TItem> values;
    public TSetData Data { get; }
    public int Count => values.Count;

    internal ImmutableDisjointHashSet(
        TSetData data,
        ImmutableHashSet<TItem> values)
    {
        Data = data;
        this.values = values;
    }

    public bool Contains(TItem item) => values.Contains(item);

    public bool SetEquals(IImmutableDisjointSet<TItem, TSetData> other)
    {
        if (ReferenceEquals(this, other))
            return true;
        return Data.Equals(other.Data) && values.SetEquals(other);
    }

    public IImmutableDisjointSetBuilder<TItem, TSetData> Add(TItem item)
    {
        var builder = values.ToBuilder();
        builder.Add(item);
        return new ImmutableDisjointHashSetBuilder<TItem, TSetData>(Data, builder);
    }

    public IImmutableDisjointSetBuilder<TItem, TSetData>? Remove(TItem item)
    {
        var builder = values.ToBuilder();
        if (!builder.Remove(item))
            throw new InvalidOperationException("Attempt to remove item that is not in the set");
        if (builder.Count == 0)
            return null;
        return new ImmutableDisjointHashSetBuilder<TItem, TSetData>(Data, builder);
    }

    public IImmutableDisjointSetBuilder<TItem, TSetData> Update(Func<TSetData, TSetData> update)
    {
        var data = update(Data);
        var builder = values.ToBuilder();
        return new ImmutableDisjointHashSetBuilder<TItem, TSetData>(data, builder);
    }

    public IEnumerator<TItem> GetEnumerator()
        // ReSharper disable once NotDisposedResourceIsReturned
        => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ImmutableDisjointHashSet<TItem, TSetData> ToImmutable() => this;

    public ImmutableDisjointHashSetBuilder<TItem, TSetData> ToBuilder()
        => new(Data, values.ToBuilder());
}
