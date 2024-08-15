using System.Collections;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

/// <summary>
/// A single disjoint set within a collection of disjoint sets.
/// </summary>
public interface IImmutableDisjointSet<TItem, TSetData> : IReadOnlyCollection<TItem>
{
    bool IsEmpty => Count == 0;

    TSetData Data { get; }

    bool Contains(TItem item);

    bool SetEquals(IImmutableDisjointSet<TItem, TSetData> other);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
