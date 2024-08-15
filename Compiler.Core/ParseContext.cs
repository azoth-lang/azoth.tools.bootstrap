using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public sealed class ParseContext
{
    public CodeFile File { get; }
    public DiagnosticCollectionBuilder Diagnostics { get; }

    public ParseContext(CodeFile file, DiagnosticCollectionBuilder diagnostics)
    {
        File = file;
        Diagnostics = diagnostics;
    }
}
