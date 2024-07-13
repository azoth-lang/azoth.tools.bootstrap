using System.Collections;
using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Framework.Collections;

/// <summary>
/// A single disjoint set within a collection of disjoint sets.
/// </summary>
public interface IImmutableDisjointSet<TItem, out TSetData> : IReadOnlyCollection<TItem>
{
    TSetData Data { get; }

    bool Contains(TItem item);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
