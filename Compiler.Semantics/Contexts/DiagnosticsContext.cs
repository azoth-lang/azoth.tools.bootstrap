using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

internal sealed class DiagnosticsContext(Diagnostics diagnostics)
{
    public Diagnostics Diagnostics { get; } = diagnostics;
}
