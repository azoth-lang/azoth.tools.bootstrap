using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

internal sealed partial class InitialContextPass
{
    private partial DiagnosticsContext EndRun(Diagnostics diagnostics);

    private partial Diagnostics Analyze(IPackageSyntax syntax);
}
