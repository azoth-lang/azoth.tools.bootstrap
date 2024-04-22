using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class CompilationUnitRemover : ITransformPass<From.Package, Void, To.Package, Void>
{
    public static To.Package Run(From.Package from)
    {
        var pass = new CompilationUnitRemover();
        pass.StartRun();
        var to = pass.Transform(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void context)
        => (Run(from), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package Transform(From.Package from);

    private partial IFixedList<To.NamespaceMemberDeclaration> Transform(From.CompilationUnit from);

    private partial IFixedSet<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.CompilationUnit> from);

    private partial IFixedSet<To.NamespaceMemberDeclaration> Transform(From.NamespaceDeclaration from, CodeFile file);

    private partial IFixedSet<To.NamespaceMemberDeclaration> Transform(From.NamespaceMemberDeclaration from, CodeFile file);

    private To.TypeDeclaration Transform(From.TypeDeclaration from, CodeFile file)
        => from switch
        {
            From.ClassDeclaration f => Transform(f, file),
            From.StructDeclaration f => Transform(f, file),
            From.TraitDeclaration f => Transform(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration Transform(From.ClassDeclaration from, CodeFile file)
        => Create(from, file);

    private To.StructDeclaration Transform(From.StructDeclaration from, CodeFile file)
        => Create(from, file);

    private To.TraitDeclaration Transform(From.TraitDeclaration from, CodeFile file)
        => Create(from, file);

    private To.FunctionDeclaration Transform(From.FunctionDeclaration from, CodeFile file)
        => Create(from, file);

    private IFixedList<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.NamespaceMemberDeclaration> from, CodeFile file)
        => from.SelectMany(f => Transform(f, file)).ToFixedList();

    private IFixedList<To.ClassMemberDeclaration> Transform(IEnumerable<From.ClassMemberDeclaration> from, CodeFile file)
        => from.Select(f => Transform(f, file)).ToFixedList();

    private To.ClassMemberDeclaration Transform(From.ClassMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.StructMemberDeclaration> Transform(IEnumerable<From.StructMemberDeclaration> from, CodeFile file)
        => from.Select(f => Transform(f, file)).ToFixedList();

    private To.StructMemberDeclaration Transform(From.StructMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TraitMemberDeclaration> Transform(IEnumerable<From.TraitMemberDeclaration> from, CodeFile file)
        => from.Select(f => Transform(f, file)).ToFixedList();

    private To.TraitMemberDeclaration Transform(From.TraitMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.Package Create(From.Package from, IEnumerable<To.NamespaceMemberDeclaration> declarations, IEnumerable<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private To.FunctionDeclaration Create(From.FunctionDeclaration from, CodeFile file)
        => To.FunctionDeclaration.Create(from.ContainingNamespace, from.Syntax, file, from.ContainingScope);

    private To.ClassDeclaration Create(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, CodeFile file)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    private To.StructDeclaration Create(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, CodeFile file)
        => To.StructDeclaration.Create(from.Syntax, members, from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    private To.TraitDeclaration Create(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, CodeFile file)
        => To.TraitDeclaration.Create(from.Syntax, members, from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

}
