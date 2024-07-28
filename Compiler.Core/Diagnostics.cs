using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

[DebuggerDisplay("Count = {items.Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public class Diagnostics : IReadOnlyCollection<Diagnostic>
{
    private readonly List<Diagnostic> items = new List<Diagnostic>();

    public Diagnostics() { }

    public Diagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        items.AddRange(diagnostics);
    }

    public int Count => items.Count;

    public void Add(Diagnostic diagnostic) => items.Add(diagnostic);

    public void Add(IEnumerable<Diagnostic> diagnostics) => items.AddRange(diagnostics);

    public IFixedList<Diagnostic> Build()
    {
        items.Sort((d1, d2) => d1.StartPosition.CompareTo(d2.StartPosition));
        return items.ToFixedList();
    }

    public IEnumerator<Diagnostic> GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items).GetEnumerator();
}

public static class DiagnosticsExtensions
{
    public static void ThrowIfFatalErrors(this Diagnostics items)
    {
        if (items.Any(i => i.IsFatal))
            throw new FatalCompilationErrorException(items.Build());
    }
}
