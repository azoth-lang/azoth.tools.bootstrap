using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

internal sealed partial class SyntaxBinder : ITransformPass<IPackageSyntax, DiagnosticsContext, Concrete.Package, SymbolBuilderContext>
{
    public static (Concrete.Package, SymbolBuilderContext) Run(IPackageSyntax from, DiagnosticsContext context)
    {
        var pass = new SyntaxBinder(context);
        pass.StartRun();
        var to = pass.Transform(from);
        return (to, pass.EndRun(to));
    }

    partial void StartRun();

    private partial SymbolBuilderContext EndRun(Concrete.Package package);

    private partial Concrete.Package Transform(IPackageSyntax from);

    private IFixedSet<Concrete.CompilationUnit> Transform(IEnumerable<ICompilationUnitSyntax> from)
        => from.Select(Transform).ToFixedSet();

    private partial Concrete.CompilationUnit Transform(ICompilationUnitSyntax from);
}
