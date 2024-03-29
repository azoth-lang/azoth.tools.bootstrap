using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Framework;

/// <summary>
/// This class is used in place of the FixedList[T] class by the debugger for
/// display purposes. It just exposes the collection items as an array at the root.
/// </summary>
public sealed class CollectionDebugView<T>
{
    private readonly IEnumerable<T> collection;

    public CollectionDebugView(IEnumerable<T> collection)
    {
        this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => collection.ToArray();
}
