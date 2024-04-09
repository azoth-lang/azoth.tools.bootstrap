using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;
using static Azoth.Tools.Bootstrap.Compiler.IST.Concrete;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

using AST = Compiler.AST;

/// <summary>
/// A nanopass that constructs the IST from the CST and binds it to the CST.
/// </summary>
public sealed class SyntaxBinderPass
{
    public static Package Build(PackageSyntax<AST.Package> syntax)
    {
        var symbol = new PackageSymbol(syntax.Symbol.Name);
        var references = syntax.References.Select(Build);
        var compilationUnits = syntax.CompilationUnits.Select(Build);
        var testingCompilationUnits = syntax.TestingCompilationUnits.Select(Build);
        return Package.Create(syntax, symbol, references, compilationUnits, testingCompilationUnits);
    }

    private static PackageReference Build(KeyValuePair<IdentifierName, AST.Package> reference)
        => PackageReference.Create(reference.Key, reference.Value);

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
