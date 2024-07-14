using System;
using System.Collections;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

internal interface IImmutableDisjointSetBuilder<TItem, TSetData> : IReadOnlyCollection<TItem>
    where TItem : notnull
    where TSetData : IMergeable<TSetData>, IEquatable<TSetData>
{
    public TSetData Data { get; }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IImmutableDisjointSetBuilder<TItem, TSetData> Add(TItem item);

    IImmutableDisjointSetBuilder<TItem, TSetData>? Remove(TItem item);

    ImmutableDisjointHashSet<TItem, TSetData> ToImmutable();
}
