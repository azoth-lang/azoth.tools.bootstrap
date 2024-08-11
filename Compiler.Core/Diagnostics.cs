using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public sealed class Diagnostics : IReadOnlyList<Diagnostic>
{
    public static readonly Diagnostics Empty = new();

    private readonly IReadOnlyList<Diagnostic> diagnostics;

    public int Count => diagnostics.Count;
    public int FatalErrorCount { get; }

    public Diagnostics(DiagnosticsBuilder diagnostics)
    {
        FatalErrorCount = diagnostics.FatalErrorCount;
        // Don't wrap in read only list, as this class is the wrapper
        this.diagnostics = diagnostics.OrderBy(d => d.File)
                                      .ThenBy(d => d.StartPosition).ThenBy(d => d.EndPosition)
                                      .ToList();
    }

    private Diagnostics()
    {
        diagnostics = Array.Empty<Diagnostic>();
    }

    public Diagnostic this[int index] => diagnostics[index];

    public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => diagnostics.GetEnumerator();
}
