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
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.Concrete;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class NamespaceSymbolBuilder : ITransformPass<From.Package, SymbolBuilderContext, To.Package, Void>
{
    public static To.Package Run(From.Package from, SymbolBuilderContext context)
    {
        var pass = new NamespaceSymbolBuilder(context);
        pass.StartRun();
        var to = pass.TransformPackage(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, SymbolBuilderContext, To.Package, Void>.Run(From.Package from, SymbolBuilderContext context)
        => (Run(from, context), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package TransformPackage(From.Package from);

    private partial To.CompilationUnit TransformCompilationUnit(From.CompilationUnit from, PackageSymbol containingNamespace, ISymbolTreeBuilder treeBuilder);

    private partial To.FunctionDeclaration TransformFunctionDeclaration(From.FunctionDeclaration from, NamespaceSymbol containingNamespace);

    private partial To.NamespaceDeclaration TransformNamespaceDeclaration(From.NamespaceDeclaration from, NamespaceSymbol containingNamespace, ISymbolTreeBuilder treeBuilder);

    private partial To.TypeDeclaration TransformTypeDeclaration(From.TypeDeclaration from, NamespaceSymbol? containingNamespace);

    private IFixedSet<To.CompilationUnit> TransformCompilationUnits(IEnumerable<From.CompilationUnit> from, PackageSymbol containingNamespace, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => TransformCompilationUnit(f, containingNamespace, treeBuilder)).ToFixedSet();

    private To.NamespaceMemberDeclaration TransformNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, NamespaceSymbol containingNamespace, ISymbolTreeBuilder treeBuilder)
        => from switch
        {
            From.NamespaceDeclaration f => TransformNamespaceDeclaration(f, containingNamespace, treeBuilder),
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            From.FunctionDeclaration f => TransformFunctionDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclarations(IEnumerable<From.NamespaceMemberDeclaration> from, NamespaceSymbol containingNamespace, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => TransformNamespaceMemberDeclaration(f, containingNamespace, treeBuilder)).ToFixedList();

    private To.ClassMemberDeclaration TransformClassMemberDeclaration(From.ClassMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.ClassMemberDeclaration> TransformClassMemberDeclarations(IEnumerable<From.ClassMemberDeclaration> from, NamespaceSymbol? containingNamespace)
        => from.Select(f => TransformClassMemberDeclaration(f, containingNamespace)).ToFixedList();

    private To.StructMemberDeclaration TransformStructMemberDeclaration(From.StructMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.StructMemberDeclaration> TransformStructMemberDeclarations(IEnumerable<From.StructMemberDeclaration> from, NamespaceSymbol? containingNamespace)
        => from.Select(f => TransformStructMemberDeclaration(f, containingNamespace)).ToFixedList();

    private To.TraitMemberDeclaration TransformTraitMemberDeclaration(From.TraitMemberDeclaration from, NamespaceSymbol? containingNamespace)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingNamespace),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TraitMemberDeclaration> TransformTraitMemberDeclarations(IEnumerable<From.TraitMemberDeclaration> from, NamespaceSymbol? containingNamespace)
        => from.Select(f => TransformTraitMemberDeclaration(f, containingNamespace)).ToFixedList();

    #region Create() methods
    private To.NamespaceDeclaration CreateNamespaceDeclaration(From.NamespaceDeclaration from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.NamespaceDeclaration.Create(containingNamespace, symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations);

    private To.FunctionDeclaration CreateFunctionDeclaration(From.FunctionDeclaration from, NamespaceSymbol containingNamespace)
        => To.FunctionDeclaration.Create(containingNamespace, from.Syntax);

    private To.Package CreatePackage(From.Package from, IEnumerable<To.CompilationUnit> compilationUnits, IEnumerable<To.CompilationUnit> testingCompilationUnits)
        => To.Package.Create(from.Syntax, from.Symbol, from.References, compilationUnits, testingCompilationUnits);

    private To.CompilationUnit CreateCompilationUnit(From.CompilationUnit from, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, declarations);

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, NamespaceSymbol? containingNamespace)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, from.GenericParameters, from.SupertypeNames, containingNamespace);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, NamespaceSymbol? containingNamespace)
        => To.StructDeclaration.Create(from.Syntax, members, from.GenericParameters, from.SupertypeNames, containingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, NamespaceSymbol? containingNamespace)
        => To.TraitDeclaration.Create(from.Syntax, members, from.GenericParameters, from.SupertypeNames, containingNamespace);

    #endregion

    #region CreateX() methods
    private To.Package CreatePackage(From.Package from, PackageSymbol childContainingNamespace, ISymbolTreeBuilder childTreeBuilder)
        => To.Package.Create(from.Syntax, from.Symbol, from.References, TransformCompilationUnits(from.CompilationUnits, childContainingNamespace, childTreeBuilder), TransformCompilationUnits(from.TestingCompilationUnits, childContainingNamespace, childTreeBuilder));

    private To.CompilationUnit CreateCompilationUnit(From.CompilationUnit from, NamespaceSymbol childContainingNamespace, ISymbolTreeBuilder childTreeBuilder)
        => To.CompilationUnit.Create(from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingNamespace, childTreeBuilder));

    private To.NamespaceDeclaration CreateNamespaceDeclaration(From.NamespaceDeclaration from, NamespaceSymbol containingNamespace, NamespaceSymbol symbol, NamespaceSymbol childContainingNamespace, ISymbolTreeBuilder childTreeBuilder)
        => To.NamespaceDeclaration.Create(containingNamespace, symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingNamespace, childTreeBuilder));

    #endregion
}
