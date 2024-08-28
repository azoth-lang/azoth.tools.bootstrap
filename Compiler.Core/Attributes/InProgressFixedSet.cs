using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

/// <summary>
/// This is used as a placeholder flag to indicate that the contributors set is in the process of
/// being built.
/// </summary>
internal sealed class InProgressFixedSet<T> : IFixedSet<T>
{
    #region Singleton
    public static readonly InProgressFixedSet<T> Instance = new();
    private InProgressFixedSet() { }
    #endregion

    public int Count => 0;
    public bool IsEmpty => true;
    public bool Equals(IFixedSet<object?>? other) => ReferenceEquals(this, other);

    public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
