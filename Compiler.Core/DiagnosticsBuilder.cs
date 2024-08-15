using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

// TODO rename to DiagnosticsBuilder (and maybe introduce an immutable diagnostics class)
[DebuggerDisplay("Count = {items.Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public class DiagnosticsBuilder : IReadOnlyCollection<Diagnostic>
{
    private readonly List<Diagnostic> items = new List<Diagnostic>();

    public DiagnosticsBuilder() { }

    public DiagnosticsBuilder(IEnumerable<Diagnostic> diagnostics)
    {
        items.AddRange(diagnostics.Do(UpdateFatalErrorCount));
    }

    public int Count => items.Count;

    public int FatalErrorCount { get; private set; }

    public void Add(Diagnostic diagnostic)
    {
        items.Add(diagnostic);
        UpdateFatalErrorCount(diagnostic);
    }

    public void Add(Diagnostics diagnostics)
    {
        items.AddRange(diagnostics);
        FatalErrorCount += diagnostics.FatalErrorCount;
    }

    public void Add(IEnumerable<Diagnostic> diagnostics)
        => items.AddRange(diagnostics.Do(UpdateFatalErrorCount));

    private void UpdateFatalErrorCount(Diagnostic diagnostic)
    {
        if (diagnostic.IsFatal)
            FatalErrorCount++;
    }

    public Diagnostics Build() => new(this);

    public IEnumerator<Diagnostic> GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

    public void ThrowIfFatalErrors()
    {
        if (FatalErrorCount > 0)
            throw new FatalCompilationErrorException(Build());
    }
}
