using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using To = Azoth.Tools.Bootstrap.Compiler.IST.Concrete;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class SyntaxBinder : ITransformPass<IPackageSyntax, DiagnosticsContext, To.Package, SymbolBuilderContext>
{
    public static (To.Package, SymbolBuilderContext) Run(IPackageSyntax from, DiagnosticsContext context)
    {
        var pass = new SyntaxBinder(context);
        pass.StartRun();
        var to = pass.TransformPackageSyntax(from);
        var toContext = pass.EndRun(to);
        return (to, toContext);
    }


    partial void StartRun();

    private partial SymbolBuilderContext EndRun(To.Package to);

    private partial To.Package TransformPackageSyntax(IPackageSyntax from);

    private partial To.PackageReference TransformPackageReferenceSyntax(IPackageReferenceSyntax from);

    private partial To.CompilationUnit TransformCompilationUnitSyntax(ICompilationUnitSyntax from);

    private IFixedSet<To.PackageReference> TransformPackageReferenceSyntaxes(IEnumerable<IPackageReferenceSyntax> from)
        => from.Select(f => TransformPackageReferenceSyntax(f)).ToFixedSet();

    private IFixedSet<To.CompilationUnit> TransformCompilationUnitSyntaxes(IEnumerable<ICompilationUnitSyntax> from)
        => from.Select(f => TransformCompilationUnitSyntax(f)).ToFixedSet();

/*  private IFixedSet<To.PackageReference> TransformPackageReferenceSyntaxes(IEnumerable<IPackageReferenceSyntax> from)
        => from.Select(f => TransformPackageReferenceSyntax(f)).ToFixedSet(); */

/*  private IFixedSet<To.CompilationUnit> TransformCompilationUnitSyntaxes(IEnumerable<ICompilationUnitSyntax> from)
        => from.Select(f => TransformCompilationUnitSyntax(f)).ToFixedSet(); */

}
