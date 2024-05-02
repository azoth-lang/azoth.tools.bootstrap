using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public sealed class ChildList<T> : IReadOnlyList<T>
    where T : class
{
    private readonly IReadOnlyList<Child<T>> children;

    public ChildList(IEnumerable<T> initialValues)
    {
        // Don't use `AsReadOnly` because FixedList<T> is already a wrapper. Use `ToArray` to
        // avoid allocating any more memory than necessary.
        children = initialValues.Select(x => new Child<T>(x)).ToArray();
    }

    public int Count => children.Count;

    public T this[int index] => children[index].Value;

    public IEnumerator<T> GetEnumerator() => children.Select(x => x.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class ChildList
{
    public static ChildList<T> Create<T>(IEnumerable<T> initialValues)
        where T : class
        => new(initialValues);
}
