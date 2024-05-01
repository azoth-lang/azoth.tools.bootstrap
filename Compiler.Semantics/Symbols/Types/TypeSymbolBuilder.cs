using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal partial class TypeSymbolBuilder
{
    private readonly SymbolBuilderContext context;
    private readonly Diagnostics diagnostics;

    private TypeSymbolBuilder(SymbolBuilderContext context)
    {
        this.context = context;
        diagnostics = context.Diagnostics;
    }

    private partial SymbolBuilderContext EndRun(To.Package to)
        => context;

    private partial To.Package TransformPackage(From.Package from)
    {
        var typeDeclarations = CreateTypeLookup(context.SymbolTree, from.Declarations);
        var declarations = TransformNamespaceMemberDeclarations(from.Declarations, typeDeclarations);
        var testingTypeDeclarations = CreateTypeLookup(context.TestingSymbolTree, from.Declarations.Concat(from.TestingDeclarations));
        var testingDeclarations = TransformNamespaceMemberDeclarations(from.TestingDeclarations, testingTypeDeclarations);
        return CreatePackage(from, declarations, testingDeclarations);
    }

    private partial To.TypeDeclaration TransformTypeDeclaration(
        From.TypeDeclaration from, TypeLookup typeDeclarations)
    {
        BuildTypeSymbol(from, typeDeclarations);
        return CreateTypeDeclaration(from,
            containingSymbol: from.ContainingSymbol.Result, symbol: from.Symbol.Result,
            typeDeclarations);
    }

    private void BuildTypeSymbol(
        From.TypeDeclaration type,
        TypeLookup typeDeclarations)
    {
        switch (type)
        {
            default:
                throw ExhaustiveMatch.Failed(type);
            case From.ClassDeclaration @class:
                BuildClassSymbol(@class, typeDeclarations);
                break;
            case From.StructDeclaration @struct:
                BuildStructSymbol(@struct, typeDeclarations);
                break;
            case From.TraitDeclaration trait:
                BuildTraitSymbol(trait, typeDeclarations);
                break;
        }
    }

    private void BuildClassSymbol(From.ClassDeclaration @class, TypeLookup typeDeclarations)
    {
        if (!@class.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

        var packageName = @class.ContainingSymbol.Result.Package!.Name;
        var typeParameters = BuildGenericParameterTypes(@class);
        var genericParameterSymbols = BuildGenericParameterSymbols(@class, typeParameters).ToFixedList();

        var superTypes = new AcyclicPromise<IFixedSet<BareReferenceType>>();
        var classType = ObjectType.CreateClass(packageName, @class.Syntax.ContainingNamespaceName, @class.IsAbstract,
            @class.Syntax.IsConst, @class.Syntax.Name, typeParameters, superTypes);

        var classSymbol = new UserTypeSymbol(@class.Syntax.ContainingNamespaceSymbol, classType);
        @class.Symbol.Fulfill(classSymbol);

        // TODO remove properties on Syntax nodes
        @class.Syntax.Symbol.BeginFulfilling();
        @class.Syntax.Symbol.Fulfill(classSymbol);

        BuildSupertypes(@class.Syntax, superTypes, typeDeclarations);

        typeDeclarations.Add(classSymbol);

        typeDeclarations.Add(genericParameterSymbols);
        // TODO move to a another pass
        @class.Syntax.CreateDefaultConstructor(typeDeclarations.SymbolTree);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(OtherSemanticError.CircularDefinition(@class.File, @class.Syntax.NameSpan, @class.Syntax));
        }
    }

    private void BuildStructSymbol(From.StructDeclaration @struct, TypeLookup typeDeclarations)
    {
        if (!@struct.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

        var packageName = @struct.Syntax.ContainingNamespaceSymbol.Package.Name;
        var typeParameters = BuildGenericParameterTypes(@struct);
        var genericParameterSymbols = BuildGenericParameterSymbols(@struct, typeParameters).ToFixedList();

        var superTypes = new AcyclicPromise<IFixedSet<BareReferenceType>>();
        var structType = StructType.Create(packageName, @struct.Syntax.ContainingNamespaceName,
            @struct.Syntax.IsConst, @struct.Syntax.Name, typeParameters, superTypes);

        var structSymbol = new UserTypeSymbol(@struct.Syntax.ContainingNamespaceSymbol, structType);
        @struct.Symbol.Fulfill(structSymbol);

        // TODO remove properties on Syntax nodes
        @struct.Syntax.Symbol.BeginFulfilling();
        @struct.Syntax.Symbol.Fulfill(structSymbol);

        BuildSupertypes(@struct.Syntax, superTypes, typeDeclarations);

        typeDeclarations.Add(structSymbol);

        typeDeclarations.Add(genericParameterSymbols);
        // TODO move to a another pass
        @struct.Syntax.CreateDefaultInitializer(typeDeclarations.SymbolTree);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(OtherSemanticError.CircularDefinition(@struct.File, @struct.Syntax.NameSpan, @struct.Syntax));
        }
    }

    private void BuildTraitSymbol(From.TraitDeclaration trait, TypeLookup typeDeclarations)
    {
        if (!trait.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

        var packageName = trait.Syntax.ContainingNamespaceSymbol.Package.Name;
        var typeParameters = BuildGenericParameterTypes(trait);
        var genericParameterSymbols = BuildGenericParameterSymbols(trait, typeParameters).ToFixedList();

        var superTypes = new AcyclicPromise<IFixedSet<BareReferenceType>>();
        var traitType = ObjectType.CreateTrait(packageName, trait.Syntax.ContainingNamespaceName,
            trait.Syntax.IsConst, trait.Syntax.Name, typeParameters, superTypes);

        var traitSymbol = new UserTypeSymbol(trait.Syntax.ContainingNamespaceSymbol, traitType);
        trait.Symbol.Fulfill(traitSymbol);

        // TODO remove properties on Syntax nodes
        trait.Syntax.Symbol.BeginFulfilling();
        trait.Syntax.Symbol.Fulfill(traitSymbol);

        BuildSupertypes(trait.Syntax, superTypes, typeDeclarations);

        typeDeclarations.Add(traitSymbol);

        typeDeclarations.Add(genericParameterSymbols);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(OtherSemanticError.CircularDefinition(trait.File, trait.Syntax.NameSpan, trait.Syntax));
        }
    }

    private static IEnumerable<From.TypeDeclaration> TypeDeclarations(IEnumerable<From.NamespaceMemberDeclaration> declarations)
        => declarations
           .OfType<From.TypeDeclaration>()
           .SelectMany(t => MoreEnumerable.TraverseDepthFirst(t, c => c.Members.OfType<From.TypeDeclaration>()));

    private TypeLookup CreateTypeLookup(
        ISymbolTreeBuilder symbolTree,
        IEnumerable<From.NamespaceMemberDeclaration> declarations)
        => new(this, symbolTree, TypeDeclarations(declarations).ToFixedDictionary(t => (IPromise<UserTypeSymbol>)t.Symbol));

    private static IFixedList<GenericParameterType> BuildGenericParameterTypes(From.TypeDeclaration type)
    {
        var declaredType = new Promise<IDeclaredUserType>();
        return type.GenericParameters.Select(p => new GenericParameterType(declaredType,
            new GenericParameter(p.Constraint.Constraint, p.Name, p.Independence, p.Variance))).ToFixedList();
    }

    private static IEnumerable<GenericParameterTypeSymbol> BuildGenericParameterSymbols(
        From.TypeDeclaration typeDeclaration,
        IFixedList<GenericParameterType> genericParameterTypes)
    {
        var typeSymbol = typeDeclaration.Symbol;
        foreach (var (genericParameter, type) in typeDeclaration.GenericParameters.EquiZip(genericParameterTypes))
        {
            var genericParameterSymbol = new GenericParameterTypeSymbol(typeSymbol, type);
            genericParameter.Symbol.Fulfill(genericParameterSymbol);
            // TODO remove properties on Syntax nodes
            genericParameter.Syntax.Symbol.Fulfill(genericParameterSymbol);
            yield return genericParameterSymbol;
        }
    }

    private void BuildSupertypes(
        ITypeDeclarationSyntax syn,
        AcyclicPromise<IFixedSet<BareReferenceType>> supertypes,
        TypeLookup typeDeclarations)
    {
        if (!supertypes.TryBeginFulfilling(AddCircularDefinitionError)) return;

        supertypes.Fulfill(EvaluateSupertypes(syn, typeDeclarations).ToFixedSet());
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(OtherSemanticError.CircularDefinition(syn.File, syn.NameSpan, syn));
        }
    }

    private IEnumerable<BareReferenceType> EvaluateSupertypes(
        ITypeDeclarationSyntax syn,
        TypeLookup typeDeclarations)
    {
        // Everything has `Any` as a supertype
        yield return BareType.Any;

        var resolver = new TypeResolver(syn.File, diagnostics, selfType: null, typeDeclarations);
        if (syn is IClassDeclarationSyntax { BaseTypeName: not null and var baseTypeName })
        {
            var superType = resolver.Evaluate(baseTypeName);
            if (superType is not null)
            {
                yield return superType;
                foreach (var inheritedType in superType.Supertypes) yield return inheritedType;
            }
        }

        foreach (var supertype in syn.SupertypeNames)
        {
            var superType = resolver.Evaluate(supertype);
            if (superType is not null)
            {
                yield return superType;
                foreach (var inheritedType in superType.Supertypes) yield return inheritedType;
            }
        }
    }

    private struct TypeLookup : ITypeSymbolBuilder
    {
        private readonly TypeSymbolBuilder typeSymbolBuilder;
        public ISymbolTreeBuilder SymbolTree { get; }
        private readonly FixedDictionary<IPromise<UserTypeSymbol>, From.TypeDeclaration> typeDeclarations;
        public TypeLookup(
            TypeSymbolBuilder typeSymbolBuilder,
            ISymbolTreeBuilder symbolTree,
            FixedDictionary<IPromise<UserTypeSymbol>, From.TypeDeclaration> typeDeclarations)
        {
            this.typeDeclarations = typeDeclarations;
            this.SymbolTree = symbolTree;
            this.typeSymbolBuilder = typeSymbolBuilder;
        }

        public TSymbol Build<TSymbol>(IPromise<TSymbol> promise)
            where TSymbol : TypeSymbol
        {
            if (promise.IsFulfilled) return promise.Result;
            if (promise is not IPromise<UserTypeSymbol> userTypePromise)
                throw new InvalidOperationException($"Only {nameof(UserTypeSymbol)}s are supported");
            var typeDeclaration = typeDeclarations[userTypePromise];
            typeSymbolBuilder.BuildTypeSymbol(typeDeclaration, this);
            return promise.Result;
        }

        public void Add(UserTypeSymbol symbol) => SymbolTree.Add(symbol);

        public void Add(IEnumerable<GenericParameterTypeSymbol> symbols) => SymbolTree.Add(symbols);
    }
}
