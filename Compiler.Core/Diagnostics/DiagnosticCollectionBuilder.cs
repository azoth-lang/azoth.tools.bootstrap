using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

[DebuggerDisplay("Count = {items.Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public class DiagnosticCollectionBuilder : IReadOnlyCollection<Diagnostic>
{
    private readonly List<Diagnostic> items = new List<Diagnostic>();

    public int Count => items.Count;

    public int FatalErrorCount { get; private set; }

    public void Add(Diagnostic diagnostic)
    {
        items.Add(diagnostic);
        UpdateFatalErrorCount(diagnostic);
    }

    public void Add(DiagnosticCollection diagnostics)
    {
        items.AddRange(diagnostics);
        FatalErrorCount += diagnostics.FatalErrorCount;
    }

    public void Add(IEnumerable<DiagnosticCollection> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            items.AddRange(diagnostic);
            FatalErrorCount += diagnostic.FatalErrorCount;
        }
    }

    public void Add(IEnumerable<Diagnostic> diagnostics)
        => items.AddRange(diagnostics.Pipe(UpdateFatalErrorCount));

    private void UpdateFatalErrorCount(Diagnostic diagnostic)
    {
        if (diagnostic.IsFatal)
            FatalErrorCount++;
    }

    public DiagnosticCollection Build() => new(this);

    public IEnumerator<Diagnostic> GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

    public void ThrowIfFatalErrors()
    {
        if (FatalErrorCount > 0)
            throw new FatalCompilationErrorException(Build());
    }
}
