using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.IST.Concrete;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;
/// <summary>
/// A nanopass that constructs the IST from the CST and binds it to the CST.
/// </summary>
internal sealed partial class SyntaxBinder
{
    private readonly Diagnostics diagnostics;

    private SyntaxBinder(DiagnosticsContext context)
    {
        diagnostics = context.Diagnostics;
    }

    private partial SymbolBuilderContext EndRun(Package package)
    {
        var packageSymbol = package.Symbol;
        return new SymbolBuilderContext(diagnostics);
    }

    private partial Package Transform(IPackageSyntax from)
    {
        var symbol = new PackageSymbol(from.Name);
        var references = from.References.Select(Build);
        var compilationUnits = from.CompilationUnits.Select(Build);
        var testingCompilationUnits = from.TestingCompilationUnits.Select(Build);
        return Package.Create(from, symbol, references, compilationUnits, testingCompilationUnits);
    }

    private static PackageReference Build(IPackageReferenceSyntax reference)
        => PackageReference.Create(reference, reference.AliasOrName, reference.Package, reference.IsTrusted);

    private static CompilationUnit Build(ICompilationUnitSyntax syntax)
    {
        var usingDirectives = syntax.UsingDirectives.Select(Build);
        var declarations = syntax.Declarations.Select(Build);
        return CompilationUnit.Create(syntax, syntax.File, syntax.ImplicitNamespaceName, usingDirectives, declarations);
    }

    private static UsingDirective Build(IUsingDirectiveSyntax syntax)
        => UsingDirective.Create(syntax, syntax.Name);

    private static NamespaceMemberDeclaration Build(INonMemberDeclarationSyntax syntax)
        => syntax switch
        {
            INamespaceDeclarationSyntax syn => Build(syn),
            ITypeDeclarationSyntax syn => Build(syn),
            IFunctionDeclarationSyntax syn => Build(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static NamespaceDeclaration Build(INamespaceDeclarationSyntax syntax)
    {
        var usingDirectives = syntax.UsingDirectives.Select(Build);
        var declarations = syntax.Declarations.Select(Build);
        return NamespaceDeclaration.Create(syntax, usingDirectives, declarations);
    }

    private static FunctionDeclaration Build(IFunctionDeclarationSyntax syntax)
        => FunctionDeclaration.Create(syntax);

    private static TypeDeclaration Build(ITypeDeclarationSyntax syntax)
        => syntax switch
        {
            IClassDeclarationSyntax syn => Build(syn),
            IStructDeclarationSyntax syn => Build(syn),
            ITraitDeclarationSyntax syn => Build(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static ClassDeclaration Build(IClassDeclarationSyntax syntax)
    {
        var isAbstract = syntax.AbstractModifier is not null;
        var members = Enumerable.Empty<ClassMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        return ClassDeclaration.Create(syntax, isAbstract, members);
    }

    private static ClassMemberDeclaration Build(IClassMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();

    private static StructDeclaration Build(IStructDeclarationSyntax syntax)
    {
        var members = Enumerable.Empty<StructMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        return StructDeclaration.Create(syntax, members);
    }

    private static StructMemberDeclaration Build(IStructMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();

    private static TraitDeclaration Build(ITraitDeclarationSyntax syntax)
    {
        var members = Enumerable.Empty<TraitMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        return TraitDeclaration.Create(syntax, members);
    }

    private static TraitMemberDeclaration Build(ITraitMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();
}
