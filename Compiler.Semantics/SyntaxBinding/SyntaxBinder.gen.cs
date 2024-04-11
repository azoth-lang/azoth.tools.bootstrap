using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    private IFixedSet<Concrete.PackageReference> Transform(IEnumerable<IPackageReferenceSyntax> from)
        => from.Select(Transform).ToFixedSet();

    private partial Concrete.PackageReference Transform(IPackageReferenceSyntax from);

    private IFixedSet<Concrete.CompilationUnit> Transform(IEnumerable<ICompilationUnitSyntax> from)
        => from.Select(Transform).ToFixedSet();

    private partial Concrete.CompilationUnit Transform(ICompilationUnitSyntax from);

    private IEnumerable<Concrete.GenericParameter> Transform(IEnumerable<IGenericParameterSyntax> from)
        => from.Select(Transform).ToFixedSet();

    private partial Concrete.GenericParameter Transform(IGenericParameterSyntax from);

    private IEnumerable<Concrete.UnresolvedSupertypeName> Transform(IEnumerable<ISupertypeNameSyntax> from)
        => from.Select(f => Transform(f)).ToFixedSet();

    [return: NotNullIfNotNull(nameof(from))]
    private partial Concrete.UnresolvedSupertypeName? Transform(ISupertypeNameSyntax? from);

    private IEnumerable<Concrete.UnresolvedType> Transform(IEnumerable<ITypeSyntax> from)
        => from.Select(Transform).ToFixedSet();

    private partial Concrete.UnresolvedType Transform(ITypeSyntax from);
}
