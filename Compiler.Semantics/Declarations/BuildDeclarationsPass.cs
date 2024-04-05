using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;

public sealed partial class BuildDeclarationsPass
{
    private readonly Diagnostics diagnostics;

    private BuildDeclarationsPass(DiagnosticsContext context)
    {
        diagnostics = context.Diagnostics;
    }

    private partial DeclarationsContext EndRun(Scoped.Package package, DeclarationTree declarations)
        => new(diagnostics, declarations);

    private partial (Scoped.Package, DeclarationTree) Transform(Concrete.Package from)
        => throw new NotImplementedException();
}
