using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

public sealed class DiagnosticsCollection : IReadOnlyList<Diagnostic>
{
    public static readonly DiagnosticsCollection Empty = new();

    private readonly IReadOnlyList<Diagnostic> diagnostics;

    public int Count => diagnostics.Count;
    public int FatalErrorCount { get; }

    public DiagnosticsCollection(DiagnosticsBuilder diagnostics)
    {
        FatalErrorCount = diagnostics.FatalErrorCount;
        // Don't wrap in read only list, as this class is the wrapper
        this.diagnostics = diagnostics.OrderBy(d => d.File)
                                      .ThenBy(d => d.StartPosition).ThenBy(d => d.EndPosition)
                                      .ToList();
    }

    private DiagnosticsCollection()
    {
        diagnostics = [];
    }

    public Diagnostic this[int index] => diagnostics[index];

    public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => diagnostics.GetEnumerator();

    public void ThrowIfFatalErrors()
    {
        if (FatalErrorCount > 0)
            throw new FatalCompilationErrorException(this);
    }
}
