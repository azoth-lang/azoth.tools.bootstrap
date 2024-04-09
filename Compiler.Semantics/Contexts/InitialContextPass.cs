using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

/// <summary>
/// This pass builds up the initial context for later passes to use.
/// </summary>
/// <remarks>For now, this largely just pulls from the CST. Once fully transitioned to nanopass,
/// this will do the work itself instead.</remarks>
internal sealed partial class InitialContextPass
{
    private readonly PackageSyntax<Package> context;

    // TODO eliminate the `PackageSyntax<Package>` context
    private InitialContextPass(PackageSyntax<Package> context)
    {
        this.context = context;
    }

    private partial DiagnosticsContext EndRun(Diagnostics diagnostics)
        // TODO use `diagnostics` parameter when nanopass is fully transitioned
        => new(context.Diagnostics);

    private partial Diagnostics Analyze(IPackageSyntax syntax)
        => new(syntax.CompilationUnits.SelectMany(cu => cu.Diagnostics));
}
