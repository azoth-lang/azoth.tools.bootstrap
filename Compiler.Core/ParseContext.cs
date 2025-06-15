using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

/// <summary>
/// A context for lexing and parsing used to know what file diagnostics should be reported against
/// and to collect diagnostics.
/// </summary>
public sealed class ParseContext
{
    public CodeFile File { [DebuggerStepThrough] get; }
    public DiagnosticCollectionBuilder Diagnostics { [DebuggerStepThrough] get; } = new();

    // TODO the DiagnosticCollectionBuilder is always a new one, any reason to pass in?
    public ParseContext(CodeFile file)
    {
        File = file;
    }
}
