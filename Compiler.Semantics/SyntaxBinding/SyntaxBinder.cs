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
/// Constructs the IST from the CST and binds it to the CST.
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
        var references = from.References.Select(Transform);
        var compilationUnits = Transform(from.CompilationUnits);
        var testingCompilationUnits = Transform(from.TestingCompilationUnits);
        return Package.Create(from, symbol, references, compilationUnits, testingCompilationUnits);
    }

    private static PackageReference Transform(IPackageReferenceSyntax reference)
        => PackageReference.Create(reference, reference.AliasOrName, reference.Package, reference.IsTrusted);

    private partial CompilationUnit Transform(ICompilationUnitSyntax from)
    {
        var usingDirectives = from.UsingDirectives.Select(Transform);
        var declarations = from.Declarations.Select(Transform);
        return CompilationUnit.Create(from, from.File, from.ImplicitNamespaceName, usingDirectives, declarations);
    }

    private static UsingDirective Transform(IUsingDirectiveSyntax syntax)
        => UsingDirective.Create(syntax, syntax.Name);

    private static NamespaceMemberDeclaration Transform(INonMemberDeclarationSyntax syntax)
        => syntax switch
        {
            INamespaceDeclarationSyntax syn => Transform(syn),
            ITypeDeclarationSyntax syn => Transform(syn),
            IFunctionDeclarationSyntax syn => Transform(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static NamespaceDeclaration Transform(INamespaceDeclarationSyntax syntax)
    {
        var usingDirectives = syntax.UsingDirectives.Select(Transform);
        var declarations = syntax.Declarations.Select(Transform);
        return NamespaceDeclaration.Create(syntax, usingDirectives, declarations);
    }

    private static FunctionDeclaration Transform(IFunctionDeclarationSyntax syntax)
        => FunctionDeclaration.Create(syntax);

    private static TypeDeclaration Transform(ITypeDeclarationSyntax syntax)
        => syntax switch
        {
            IClassDeclarationSyntax syn => Transform(syn),
            IStructDeclarationSyntax syn => Transform(syn),
            ITraitDeclarationSyntax syn => Transform(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static ClassDeclaration Transform(IClassDeclarationSyntax syntax)
    {
        var isAbstract = syntax.AbstractModifier is not null;
        var members = Enumerable.Empty<ClassMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        return ClassDeclaration.Create(syntax, isAbstract, members);
    }

    private static ClassMemberDeclaration Transform(IClassMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();

    private static StructDeclaration Transform(IStructDeclarationSyntax syntax)
    {
        var members = Enumerable.Empty<StructMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        return StructDeclaration.Create(syntax, members);
    }

    private static StructMemberDeclaration Transform(IStructMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();

    private static TraitDeclaration Transform(ITraitDeclarationSyntax syntax)
    {
        var members = Enumerable.Empty<TraitMemberDeclaration>(); // TODO syntax.Members.Select(Build);
        return TraitDeclaration.Create(syntax, members);
    }

    private static TraitMemberDeclaration Transform(ITraitMemberDeclarationSyntax syntax)
        => throw new NotImplementedException();
}
