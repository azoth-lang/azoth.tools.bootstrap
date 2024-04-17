using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

internal sealed partial class InitialContextPass : IAnalyzePass<IPackageSyntax, PackageSyntax<Package>, DiagnosticsContext>
{
    public static DiagnosticsContext Run(IPackageSyntax value, PackageSyntax<Package> context)
    {
        var pass = new InitialContextPass(context);
        pass.StartRun();
        var diagnostics = pass.Analyze(value);
        return pass.EndRun(diagnostics);
    }

    static (IPackageSyntax, DiagnosticsContext) ITransformPass<IPackageSyntax, PackageSyntax<Package>, IPackageSyntax, DiagnosticsContext>.Run(
            IPackageSyntax from,
            PackageSyntax<Package> context)
        => (from, Run(from, context));

    partial void StartRun();

    private partial DiagnosticsContext EndRun(Diagnostics diagnostics);

    private partial Diagnostics Analyze(IPackageSyntax syntax);
}
