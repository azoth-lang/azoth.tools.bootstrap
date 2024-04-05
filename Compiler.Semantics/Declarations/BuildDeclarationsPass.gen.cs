using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;

public sealed partial class BuildDeclarationsPass : IPass<Concrete.Package, DiagnosticsContext, Scoped.Package, DeclarationsContext>
{
    public static (Scoped.Package, DeclarationsContext) Run(Concrete.Package from, DiagnosticsContext context)
    {
        var pass = new BuildDeclarationsPass(context);
        pass.StartRun();
        var (to, tree) = pass.Transform(from);
        return (to, pass.EndRun(to, tree));
    }

    partial void StartRun();

    private partial DeclarationsContext EndRun(Scoped.Package package, DeclarationTree declarations);

    private partial (Scoped.Package, DeclarationTree) Transform(Concrete.Package from);
}
