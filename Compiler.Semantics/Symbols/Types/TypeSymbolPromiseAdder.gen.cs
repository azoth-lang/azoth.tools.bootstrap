using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal sealed partial class TypeSymbolPromiseAdder : ITransformPass<From.Package, Void, To.Package, Void>
{
    public static To.Package Run(From.Package from)
    {
        var pass = new TypeSymbolPromiseAdder();
        pass.StartRun();
        var to = pass.Transform(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void context)
        => (Run(from), default);

    partial void StartRun();

    partial void EndRun(To.Package package);

    private To.Package Transform(From.Package from)
        => Create(from);

    private To.Package Create(From.Package from)
        => Create(from, Transform(from.Declarations), Transform(from.TestingDeclarations));

    private To.Package Create(From.Package from, IEnumerable<To.NamespaceMemberDeclaration> declarations,
        IEnumerable<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private IFixedSet<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.NamespaceMemberDeclaration> from)
        => from.Select(Transform).ToFixedSet();

    private To.NamespaceMemberDeclaration Transform(From.NamespaceMemberDeclaration from)
        => from switch
        {
            From.TypeDeclaration f => Transform(f),
            From.FunctionDeclaration f => f,
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private partial To.TypeDeclaration Transform(From.TypeDeclaration from);

    private To.TypeDeclaration Create(From.TypeDeclaration from, AcyclicPromise<UserTypeSymbol> symbol)
        => from switch
        {
            From.ClassDeclaration f => Create(f, symbol),
            From.TraitDeclaration f => Create(f, symbol),
            From.StructDeclaration f => Create(f, symbol),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration Create(From.ClassDeclaration from, AcyclicPromise<UserTypeSymbol> symbol)
        => Create(from, Transform(from.Members), symbol);

    private To.ClassDeclaration Create(
        From.ClassDeclaration from,
        IEnumerable<To.ClassMemberDeclaration> members,
        AcyclicPromise<UserTypeSymbol> symbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, symbol,
            from.LexicalScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingLexicalScope,
            from.ContainingSymbol);

    private IFixedList<To.ClassMemberDeclaration> Transform(IEnumerable<From.ClassMemberDeclaration> from)
        => from.Select(Transform).ToFixedList();

    private To.ClassMemberDeclaration Transform(From.ClassMemberDeclaration from)
        => from switch
        {
            From.ClassDeclaration f => Transform(f),
            From.TraitDeclaration f => Transform(f),
            From.StructDeclaration f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitDeclaration Create(From.TraitDeclaration from, AcyclicPromise<UserTypeSymbol> symbol)
        => Create(from, Transform(from.Members), symbol);

    private To.TraitDeclaration Create(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol)
        => To.TraitDeclaration.Create(from.Syntax, members, symbol, from.LexicalScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingLexicalScope, from.ContainingSymbol);

    private IFixedList<To.TraitMemberDeclaration> Transform(IEnumerable<From.TraitMemberDeclaration> from)
        => from.Select(Transform).ToFixedList();

    private To.TraitMemberDeclaration Transform(From.TraitMemberDeclaration from)
        => from switch
        {
            From.ClassDeclaration f => Transform(f),
            From.TraitDeclaration f => Transform(f),
            From.StructDeclaration f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructDeclaration Create(From.StructDeclaration from, AcyclicPromise<UserTypeSymbol> symbol)
        => Create(from, Transform(from.Members), symbol);

    private To.StructDeclaration Create(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, AcyclicPromise<UserTypeSymbol> symbol)
        => To.StructDeclaration.Create(from.Syntax, members, symbol, from.LexicalScope, from.GenericParameters, from.SupertypeNames, from.File, from.ContainingLexicalScope, from.ContainingSymbol);

    private IFixedList<To.StructMemberDeclaration> Transform(IEnumerable<From.StructMemberDeclaration> from)
        => from.Select(Transform).ToFixedList();

    private To.StructMemberDeclaration Transform(From.StructMemberDeclaration from)
        => from switch
        {
            From.ClassDeclaration f => Transform(f),
            From.TraitDeclaration f => Transform(f),
            From.StructDeclaration f => Transform(f),
            _ => throw ExhaustiveMatch.Failed(from),
        };
}
