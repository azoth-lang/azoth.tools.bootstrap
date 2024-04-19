using System.CodeDom.Compiler;
using System.Collections.Generic;
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
        var to = pass.Transform(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, SymbolBuilderContext, To.Package, Void>.Run(From.Package from, SymbolBuilderContext context)
        => (Run(from, context), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package Transform(From.Package from);

    private partial To.CompilationUnit Transform(From.CompilationUnit from, PackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder);

    private partial To.FunctionDeclaration Transform(From.FunctionDeclaration from, NamespaceSymbol containingSymbol);

    private partial To.NamespaceDeclaration Transform(From.NamespaceDeclaration from, NamespaceSymbol containingSymbol, ISymbolTreeBuilder treeBuilder);

    private partial To.TypeDeclaration Transform(From.TypeDeclaration from, NamespaceSymbol? containingSymbol);

    private IFixedSet<To.CompilationUnit> Transform(IEnumerable<From.CompilationUnit> from, PackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => Transform(f, containingSymbol, treeBuilder)).ToFixedSet();

    private IFixedList<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.NamespaceMemberDeclaration> from, NamespaceSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => Transform(f, containingSymbol, treeBuilder)).ToFixedList();

    private IFixedList<To.ClassMemberDeclaration> Transform(IEnumerable<From.ClassMemberDeclaration> from, NamespaceSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private IFixedList<To.StructMemberDeclaration> Transform(IEnumerable<From.StructMemberDeclaration> from, NamespaceSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private IFixedList<To.TraitMemberDeclaration> Transform(IEnumerable<From.TraitMemberDeclaration> from, NamespaceSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private To.NamespaceDeclaration Create(From.NamespaceDeclaration from, NamespaceSymbol containingSymbol, NamespaceSymbol symbol, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.NamespaceDeclaration.Create(containingSymbol, symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations);

    private To.FunctionDeclaration Create(From.FunctionDeclaration from, NamespaceSymbol containingSymbol)
        => To.FunctionDeclaration.Create(containingSymbol, from.Syntax);

    private To.Package Create(From.Package from, IEnumerable<To.CompilationUnit> compilationUnits, IEnumerable<To.CompilationUnit> testingCompilationUnits)
        => To.Package.Create(from.Syntax, from.Symbol, from.References, compilationUnits, testingCompilationUnits);

    private To.CompilationUnit Create(From.CompilationUnit from, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, declarations);

    private To.ClassDeclaration Create(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, NamespaceSymbol? containingSymbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, from.GenericParameters, from.SupertypeNames, containingSymbol);

    private To.StructDeclaration Create(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, NamespaceSymbol? containingSymbol)
        => To.StructDeclaration.Create(from.Syntax, members, from.GenericParameters, from.SupertypeNames, containingSymbol);

    private To.TraitDeclaration Create(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, NamespaceSymbol? containingSymbol)
        => To.TraitDeclaration.Create(from.Syntax, members, from.GenericParameters, from.SupertypeNames, containingSymbol);

}
