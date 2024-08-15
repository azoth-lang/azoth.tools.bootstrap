using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public class ParseContext
{
    public CodeFile File { get; }
    public DiagnosticsBuilder Diagnostics { get; }

    public ParseContext(CodeFile file, DiagnosticsBuilder diagnostics)
    {
        File = file;
        Diagnostics = diagnostics;
    }
}
