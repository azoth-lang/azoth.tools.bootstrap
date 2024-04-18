using System.CodeDom.Compiler;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class InitialContextPass : ITransformPass<IPackageSyntax, PackageSyntax<Package>, Void, DiagnosticsContext>
{
    public static DiagnosticsContext Run(IPackageSyntax from, PackageSyntax<Package> context)
    {
        var pass = new InitialContextPass(context);
        pass.StartRun();
        var diagnostics = pass.Analyze(from);
        var toContext = pass.EndRun(diagnostics);
        return toContext;
    }

    static (Void, DiagnosticsContext) ITransformPass<IPackageSyntax, PackageSyntax<Package>, Void, DiagnosticsContext>.Run(IPackageSyntax from, PackageSyntax<Package> context)
        => (default, Run(from, context));

}
