using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;

public class EntitySymbolBuilder
{
    private readonly Diagnostics diagnostics;
    private readonly ISymbolTreeBuilder symbolTree;
    private readonly SymbolForest symbolTrees;

    private EntitySymbolBuilder(Diagnostics diagnostics, ISymbolTreeBuilder symbolTree, SymbolForest symbolTrees)
    {
        this.diagnostics = diagnostics;
        this.symbolTree = symbolTree;
        this.symbolTrees = symbolTrees;
    }

    public static void BuildFor(PackageSyntax<Package> package)
    {
        var builder = new EntitySymbolBuilder(package.Diagnostics, package.SymbolTree, package.SymbolTrees);
        builder.Build(package.EntityDeclarations);

        builder = new EntitySymbolBuilder(package.Diagnostics, package.TestingSymbolTree, package.SymbolTrees);
        builder.Build(package.TestingEntityDeclarations);
    }

    private void Build(FixedSet<IEntityDeclarationSyntax> entities)
    {
        // Process all types first because they may be referenced by functions etc.
        var typeDeclarations = new TypeSymbolBuilder(this, entities.OfType<ITypeDeclarationSyntax>());
        foreach (var type in typeDeclarations)
            BuildTypeSymbol(type, typeDeclarations);

        // Now resolve all other symbols (type declarations will already have symbols and won't be processed again)
        foreach (var entity in entities)
            BuildNonTypeEntitySymbol(entity);

        var inheritor = new TypeSymbolInheritor(symbolTree, symbolTrees, typeDeclarations);
        inheritor.AddInheritedSymbols();
    }

    /// <summary>
    /// If the type has not been resolved, this resolves it. This function
    /// also watches for type cycles and reports an error.
    /// </summary>
    private void BuildNonTypeEntitySymbol(IEntityDeclarationSyntax entity)
    {
        switch (entity)
        {
            default:
                throw ExhaustiveMatch.Failed(entity);
            case IMethodDeclarationSyntax method:
                BuildMethodSymbol(method);
                break;
            case IConstructorDeclarationSyntax constructor:
                BuildConstructorSymbol(constructor);
                break;
            case IAssociatedFunctionDeclarationSyntax associatedFunction:
                BuildAssociatedFunctionSymbol(associatedFunction);
                break;
            case IFieldDeclarationSyntax field:
                BuildFieldSymbol(field);
                break;
            case IFunctionDeclarationSyntax syn:
                BuildFunctionSymbol(syn);
                break;
            case ITypeDeclarationSyntax _:
                // Type declarations already processed
                break;
        }
    }

    private void BuildMethodSymbol(IMethodDeclarationSyntax method)
    {
        method.Symbol.BeginFulfilling();
        var declaringTypeSymbol = method.DeclaringType.Symbol.Result;
        var file = method.File;
        var selfParameterType = ResolveMethodSelfParameterType(file, method.SelfParameter, method.DeclaringType);
        var resolver = new TypeResolver(file, diagnostics, selfParameterType.Type);
        var parameterTypes = ResolveParameterTypes(resolver, method.Parameters, method.DeclaringType);
        var returnType = ResolveReturnType(resolver, method.Return);
        var symbol = new MethodSymbol(declaringTypeSymbol, method.Name, selfParameterType, parameterTypes, returnType);
        method.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildSelfParameterSymbol(symbol, method.SelfParameter, selfParameterType.Type);
        BuildParameterSymbols(symbol, file, method.Parameters, parameterTypes);
    }

    private void BuildConstructorSymbol(IConstructorDeclarationSyntax constructor)
    {
        constructor.Symbol.BeginFulfilling();
        var file = constructor.File;
        var selfParameterType = ResolveConstructorSelfParameterType(constructor.SelfParameter, constructor.DeclaringType);
        var resolver = new TypeResolver(file, diagnostics, selfParameterType);
        var parameterTypes = ResolveParameterTypes(resolver, constructor.Parameters, constructor.DeclaringType);

        var declaringClassSymbol = constructor.DeclaringType.Symbol.Result;
        var symbol = new ConstructorSymbol(declaringClassSymbol, constructor.Name, selfParameterType, parameterTypes);
        constructor.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildSelfParameterSymbol(symbol, constructor.SelfParameter, selfParameterType, isConstructor: true);
        BuildParameterSymbols(symbol, file, constructor.Parameters, parameterTypes);
    }

    private void BuildAssociatedFunctionSymbol(IAssociatedFunctionDeclarationSyntax associatedFunction)
    {
        associatedFunction.Symbol.BeginFulfilling();
        var file = associatedFunction.File;
        var resolver = new TypeResolver(file, diagnostics, selfType: null);
        var parameterTypes = ResolveParameterTypes(resolver, associatedFunction.Parameters);
        var returnType = ResolveReturnType(resolver, associatedFunction.Return);
        var type = new FunctionType(parameterTypes, returnType);
        var declaringTypeSymbol = associatedFunction.DeclaringType.Symbol.Result;
        var symbol = new FunctionSymbol(declaringTypeSymbol, associatedFunction.Name, type);
        associatedFunction.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildParameterSymbols(symbol, file, associatedFunction.Parameters, parameterTypes);
    }

    private FieldSymbol BuildFieldSymbol(IFieldDeclarationSyntax field)
    {
        if (field.Symbol.IsFulfilled)
            return field.Symbol.Result;

        field.Symbol.BeginFulfilling();
        var resolver = new TypeResolver(field.File, diagnostics, selfType: null);
        var type = resolver.Evaluate(field.Type);
        var symbol = new FieldSymbol(field.DeclaringType.Symbol.Result, field.Name, field.IsMutableBinding, type);
        field.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        return symbol;
    }

    private void BuildFunctionSymbol(IFunctionDeclarationSyntax function)
    {
        function.Symbol.BeginFulfilling();
        var file = function.File;
        var resolver = new TypeResolver(file, diagnostics, selfType: null);
        var parameterTypes = ResolveParameterTypes(resolver, function.Parameters);
        var returnType = ResolveReturnType(resolver, function.Return);
        var type = new FunctionType(parameterTypes, returnType);
        var symbol = new FunctionSymbol(function.ContainingNamespaceSymbol, function.Name, type);
        function.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildParameterSymbols(symbol, file, function.Parameters, parameterTypes);
    }

    private void BuildTypeSymbol(ITypeDeclarationSyntax type, TypeSymbolBuilder typeDeclarations)
    {
        switch (type)
        {
            default:
                throw ExhaustiveMatch.Failed(type);
            case IClassDeclarationSyntax @class:
                BuildClassSymbol(@class, typeDeclarations);
                break;
            case ITraitDeclarationSyntax trait:
                BuildTraitSymbol(trait, typeDeclarations);
                break;
        }
    }

    private void BuildClassSymbol(IClassDeclarationSyntax @class, TypeSymbolBuilder typeDeclarations)
    {
        if (!@class.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

        var packageName = @class.ContainingNamespaceSymbol.Package.Name;
        var typeParameters = BuildGenericParameterTypes(@class);
        var genericParameterSymbols = BuildGenericParameterSymbols(@class, typeParameters).ToFixedList();

        var superTypes = new AcyclicPromise<FixedSet<BareReferenceType>>();
        var classType = DeclaredObjectType.Create(packageName, @class.ContainingNamespaceName,
            @class.IsAbstract, @class.IsConst, isClass: true, @class.Name, typeParameters, superTypes);

        var classSymbol = new ObjectTypeSymbol(@class.ContainingNamespaceSymbol, classType);
        @class.Symbol.Fulfill(classSymbol);

        BuildSupertypes(@class, superTypes, typeDeclarations);

        symbolTree.Add(classSymbol);

        symbolTree.Add(genericParameterSymbols);
        @class.CreateDefaultConstructor(symbolTree);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(OtherSemanticError.CircularDefinition(@class.File, @class.NameSpan, @class));
        }
    }

    private void BuildTraitSymbol(ITraitDeclarationSyntax trait, TypeSymbolBuilder typeDeclarations)
    {
        if (!trait.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

        var packageName = trait.ContainingNamespaceSymbol.Package.Name;
        var typeParameters = BuildGenericParameterTypes(trait);
        var genericParameterSymbols = BuildGenericParameterSymbols(trait, typeParameters).ToFixedList();

        var superTypes = new AcyclicPromise<FixedSet<BareReferenceType>>();
        var traitType = DeclaredObjectType.Create(packageName, trait.ContainingNamespaceName,
            isAbstract: true, trait.IsConst, isClass: false, trait.Name, typeParameters, superTypes);

        var traitSymbol = new ObjectTypeSymbol(trait.ContainingNamespaceSymbol, traitType);
        trait.Symbol.Fulfill(traitSymbol);

        BuildSupertypes(trait, superTypes, typeDeclarations);

        symbolTree.Add(traitSymbol);

        symbolTree.Add(genericParameterSymbols);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(OtherSemanticError.CircularDefinition(trait.File, trait.NameSpan, trait));
        }
    }

    private static FixedList<GenericParameterType> BuildGenericParameterTypes(ITypeDeclarationSyntax type)
    {
        var declaredType = new Promise<DeclaredObjectType>();
        return type.GenericParameters.Select(p => new GenericParameterType(declaredType, new GenericParameter(p.Variance, p.Name)))
                   .ToFixedList();
    }

    private static IEnumerable<GenericParameterTypeSymbol> BuildGenericParameterSymbols(
        ITypeDeclarationSyntax typeSyntax,
        FixedList<GenericParameterType> genericParameterTypes)
    {
        var typeSymbol = typeSyntax.Symbol;
        foreach (var (syn, genericParameter) in typeSyntax.GenericParameters.Zip(genericParameterTypes))
        {
            var genericParameterSymbol = new GenericParameterTypeSymbol(typeSymbol, genericParameter);
            syn.Symbol.Fulfill(genericParameterSymbol);
            yield return genericParameterSymbol;
        }
    }

    private void BuildSupertypes(
        ITypeDeclarationSyntax syn,
        AcyclicPromise<FixedSet<BareReferenceType>> supertypes,
        TypeSymbolBuilder typeDeclarations)
    {
        if (!supertypes.TryBeginFulfilling(AddCircularDefinitionError)) return;

        supertypes.Fulfill(EvaluateSupertypes(syn, typeDeclarations).ToFixedSet());

        void AddCircularDefinitionError()
        {
            diagnostics.Add(OtherSemanticError.CircularDefinition(syn.File, syn.NameSpan, syn));
        }
    }

    private IEnumerable<BareReferenceType> EvaluateSupertypes(
        ITypeDeclarationSyntax syn,
        TypeSymbolBuilder typeDeclarations)
    {
        // Everything has `Any` as a supertype
        yield return BareReferenceType.Any;

        var resolver = new TypeResolver(syn.File, diagnostics, selfType: null, typeDeclarations);
        if (syn is IClassDeclarationSyntax { BaseTypeName: not null and var baseTypeName })
        {
            var baseType = resolver.EvaluateBareType(baseTypeName);
            if (baseType is ReferenceType { BareType: var bareType })
            {
                yield return bareType;
                foreach (var inheritedType in bareType.Supertypes)
                    yield return inheritedType;
            }
        }

        foreach (var supertype in syn.SupertypeNames)
        {
            var superType = resolver.EvaluateBareType(supertype);
            if (superType is ReferenceType { BareType: var bareType })
            {
                yield return bareType;
                foreach (var inheritedType in bareType.Supertypes)
                    yield return inheritedType;
            }
        }
    }

    private static FixedList<ParameterType> ResolveParameterTypes(
        TypeResolver resolver,
        IEnumerable<INamedParameterSyntax> parameters)
    {
        var types = new List<ParameterType>();
        foreach (var parameter in parameters)
        {
            var type = resolver.Evaluate(parameter.Type);
            types.Add(new ParameterType(parameter.IsLentBinding, type));
        }

        return types.ToFixedList();
    }

    private FixedList<ParameterType> ResolveParameterTypes(
        TypeResolver resolver,
        IEnumerable<IConstructorParameterSyntax> parameters,
        ITypeDeclarationSyntax declaringType)
    {
        var types = new List<ParameterType>();
        foreach (var parameter in parameters)
            switch (parameter)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter);
                case INamedParameterSyntax namedParameter:
                {
                    var type = resolver.Evaluate(namedParameter.Type);
                    types.Add(new ParameterType(namedParameter.IsLentBinding, type));
                    break;
                }
                case IFieldParameterSyntax fieldParameter:
                {
                    var field = declaringType.Members.OfType<IFieldDeclarationSyntax>()
                                              .SingleOrDefault(f => f.Name == fieldParameter.Name);
                    if (field is null)
                    {
                        types.Add(new ParameterType(false, DataType.Unknown));
                        fieldParameter.ReferencedSymbol.Fulfill(null);
                        throw new NotImplementedException("Report diagnostic about field parameter without matching field");
                    }
                    else
                    {
                        var fieldSymbol = BuildFieldSymbol(field);
                        fieldParameter.ReferencedSymbol.Fulfill(fieldSymbol);
                        types.Add(new ParameterType(false, fieldSymbol.Type));
                    }
                    break;
                }
            }

        return types.ToFixedList();
    }

    private void BuildParameterSymbols(
        InvocableSymbol containingSymbol,
        CodeFile file,
        IEnumerable<IConstructorParameterSyntax> parameters,
        IEnumerable<ParameterType> parameterTypes)
        => BuildParameterSymbols(containingSymbol, file, parameters, parameterTypes.Select(pt => pt.Type));

    private void BuildParameterSymbols(
        InvocableSymbol containingSymbol,
        CodeFile file,
        IEnumerable<IConstructorParameterSyntax> parameters,
        IEnumerable<DataType> types)
    {
        foreach (var (param, type) in parameters.Zip(types))
        {
            switch (param)
            {
                default:
                    throw ExhaustiveMatch.Failed(param);
                case INamedParameterSyntax namedParam:
                {
                    var isLent = namedParam.IsLentBinding;
                    if (isLent && type is ReferenceType { IsIdentityReference: true })
                    {
                        diagnostics.Add(TypeError.LentConstOrIdentity(file, namedParam.Span, type));
                        isLent = false;
                    }

                    var symbol = VariableSymbol.CreateParameter(containingSymbol, namedParam.Name,
                        namedParam.DeclarationNumber.Result, namedParam.IsMutableBinding, isLent, type);
                    namedParam.Symbol.Fulfill(symbol);
                    symbolTree.Add(symbol);
                }
                break;
                case IFieldParameterSyntax _:
                    // Referenced field already assigned
                    break;
            }
        }
    }

    private ReferenceType ResolveConstructorSelfParameterType(
        IConstructorSelfParameterSyntax selfParameter,
        IClassDeclarationSyntax declaringClass)
    {
        var declaredType = declaringClass.Symbol.Result.DeclaresType;
        var resolver = new SelfTypeResolver(declaringClass.File, diagnostics);
        return resolver.EvaluateConstructorSelfParameterType(declaredType, selfParameter.Capability, declaredType.GenericParameterDataTypes);
    }

    private SelfParameterType ResolveMethodSelfParameterType(
        CodeFile file,
        IMethodSelfParameterSyntax selfParameter,
        ITypeDeclarationSyntax declaringType)
    {
        var declaredType = declaringType.Symbol.Result.DeclaresType;
        var resolver = new SelfTypeResolver(declaringType.File, diagnostics);
        var selfType = resolver.EvaluateMethodSelfParameterType(declaredType, selfParameter.Capability, declaredType.GenericParameterDataTypes);
        bool isLent = selfParameter.IsLentBinding;
        if (isLent && selfType is ReferenceType { IsIdentityReference: true })
        {
            diagnostics.Add(TypeError.LentConstOrIdentity(file, selfParameter.Span, selfType));
            isLent = false;
        }
        return new SelfParameterType(isLent, selfType);
    }

    private void BuildSelfParameterSymbol(
        InvocableSymbol containingSymbol,
        ISelfParameterSyntax param,
        Pseudotype type,
        bool isConstructor = false)
    {
        var symbol = new SelfParameterSymbol(containingSymbol, param.IsLentBinding && !isConstructor, type);
        param.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
    }

    private static ReturnType ResolveReturnType(
        TypeResolver resolver,
        IReturnSyntax? returnSyntax)
    {
        if (returnSyntax is null)
            return ReturnType.Void;
        DataType type = resolver.Evaluate(returnSyntax.Type);
        return new ReturnType(type);
    }

    private class TypeSymbolBuilder : ITypeSymbolBuilder, IEnumerable<ITypeDeclarationSyntax>
    {
        private readonly EntitySymbolBuilder symbolBuilder;
        private readonly FixedDictionary<IPromise<TypeSymbol>, ITypeDeclarationSyntax> typeDeclarations;

        public TypeSymbolBuilder(EntitySymbolBuilder symbolBuilder, IEnumerable<ITypeDeclarationSyntax> typeDeclarations)
        {
            this.symbolBuilder = symbolBuilder;
            this.typeDeclarations = typeDeclarations.ToFixedDictionary(t => (IPromise<TypeSymbol>)t.Symbol);
        }

        public TypeSymbol Build(IPromise<TypeSymbol> promise)
        {
            if (promise.IsFulfilled) return promise.Result;
            var typeDeclaration = typeDeclarations[promise];
            symbolBuilder.BuildTypeSymbol(typeDeclaration, this);
            return promise.Result;
        }

        #region IEnumerable<ITypeDeclarationSyntax>
        public IEnumerator<ITypeDeclarationSyntax> GetEnumerator() => typeDeclarations.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
