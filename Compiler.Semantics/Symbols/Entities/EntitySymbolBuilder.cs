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
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Entities;

public class EntitySymbolBuilder
{
    private readonly Diagnostics diagnostics;
    private readonly SymbolTreeBuilder symbolTree;

    private EntitySymbolBuilder(Diagnostics diagnostics, SymbolTreeBuilder symbolTree)
    {
        this.diagnostics = diagnostics;
        this.symbolTree = symbolTree;
    }

    public static void BuildFor(PackageSyntax<Package> package)
    {
        var builder = new EntitySymbolBuilder(package.Diagnostics, package.SymbolTree);
        builder.Build(package.AllEntityDeclarations);
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

        var inheritor = new TypeSymbolInheritor(symbolTree, typeDeclarations);
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
        var resolver = new TypeResolver(file, diagnostics);
        var selfParameterType = ResolveMethodSelfParameterType(resolver, method.SelfParameter, method.DeclaringType);
        var parameterTypes = ResolveParameterTypes(resolver, method.Parameters, method.DeclaringType);
        var returnType = ResolveReturnType(file, method.Return, resolver);
        var symbol = new MethodSymbol(declaringTypeSymbol, method.Name, selfParameterType, parameterTypes, returnType);
        method.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildSelfParameterSymbol(symbol, file, method.SelfParameter, selfParameterType.Type);
        BuildParameterSymbols(symbol, file, method.Parameters, parameterTypes);
    }

    private void BuildConstructorSymbol(IConstructorDeclarationSyntax constructor)
    {
        constructor.Symbol.BeginFulfilling();
        var file = constructor.File;
        var resolver = new TypeResolver(file, diagnostics);
        var selfParameterType = ResolveConstructorSelfParameterType(resolver, constructor.SelfParameter, constructor.DeclaringType);
        var parameterTypes = ResolveParameterTypes(resolver, constructor.Parameters, constructor.DeclaringType);

        var declaringClassSymbol = constructor.DeclaringType.Symbol.Result;
        var symbol = new ConstructorSymbol(declaringClassSymbol, constructor.Name, selfParameterType, parameterTypes);
        constructor.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildSelfParameterSymbol(symbol, file, constructor.SelfParameter, selfParameterType, isConstructor: true);
        BuildParameterSymbols(symbol, file, constructor.Parameters, parameterTypes);
    }

    private void BuildAssociatedFunctionSymbol(IAssociatedFunctionDeclarationSyntax associatedFunction)
    {
        associatedFunction.Symbol.BeginFulfilling();
        var file = associatedFunction.File;
        var resolver = new TypeResolver(file, diagnostics);
        var parameterTypes = ResolveParameterTypes(resolver, associatedFunction.Parameters);
        var returnType = ResolveReturnType(file, associatedFunction.Return, resolver);
        var declaringTypeSymbol = associatedFunction.DeclaringType.Symbol.Result;
        var symbol = new FunctionSymbol(declaringTypeSymbol, associatedFunction.Name, parameterTypes, returnType);
        associatedFunction.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildParameterSymbols(symbol, file, associatedFunction.Parameters, parameterTypes);
    }

    private FieldSymbol BuildFieldSymbol(IFieldDeclarationSyntax field)
    {
        if (field.Symbol.IsFulfilled)
            return field.Symbol.Result;

        field.Symbol.BeginFulfilling();
        var resolver = new TypeResolver(field.File, diagnostics);
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
        var resolver = new TypeResolver(file, diagnostics);
        var parameterTypes = ResolveParameterTypes(resolver, function.Parameters);
        var returnType = ResolveReturnType(file, function.Return, resolver);
        var symbol = new FunctionSymbol(function.ContainingNamespaceSymbol, function.Name, parameterTypes, returnType);
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

        var packageName = @class.ContainingNamespaceSymbol.Package!.Name;
        var typeParameters = BuildGenericParameterTypes(@class);
        var genericParameterSymbols = BuildGenericParameterSymbols(@class, typeParameters).ToFixedList();

        var superTypes = EvaluateSupertypes(@class, typeDeclarations).ToFixedSet();
        var classType = DeclaredObjectType.Create(packageName, @class.ContainingNamespaceName,
            @class.IsAbstract, @class.IsConst, isClass: true, @class.Name, typeParameters, superTypes);

        var classSymbol = new ObjectTypeSymbol(@class.ContainingNamespaceSymbol, classType);
        @class.Symbol.Fulfill(classSymbol);
        symbolTree.Add(classSymbol);

        symbolTree.Add(genericParameterSymbols);
        @class.CreateDefaultConstructor(symbolTree);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(TypeError.CircularDefinition(@class.File, @class.NameSpan, @class));
        }
    }

    private void BuildTraitSymbol(ITraitDeclarationSyntax trait, TypeSymbolBuilder typeDeclarations)
    {
        if (!trait.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

        var packageName = trait.ContainingNamespaceSymbol.Package!.Name;
        var typeParameters = BuildGenericParameterTypes(trait);
        var genericParameterSymbols = BuildGenericParameterSymbols(trait, typeParameters).ToFixedList();

        var superTypes = EvaluateSupertypes(trait, typeDeclarations).ToFixedSet();
        var traitType = DeclaredObjectType.Create(packageName, trait.ContainingNamespaceName,
            isAbstract: true, trait.IsConst, isClass: false, trait.Name, typeParameters, superTypes);

        var traitSymbol = new ObjectTypeSymbol(trait.ContainingNamespaceSymbol, traitType);
        trait.Symbol.Fulfill(traitSymbol);
        symbolTree.Add(traitSymbol);

        symbolTree.Add(genericParameterSymbols);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(TypeError.CircularDefinition(trait.File, trait.NameSpan, trait));
        }
    }

    private static FixedList<GenericParameterType> BuildGenericParameterTypes(ITypeDeclarationSyntax type)
    {
        var declaredType = new Promise<DeclaredObjectType>();
        return type.GenericParameters.Select(p => new GenericParameterType(declaredType, new GenericParameter(p.Name)))
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

    private IEnumerable<DeclaredObjectType> EvaluateSupertypes(
        ITypeDeclarationSyntax syn,
        TypeSymbolBuilder typeDeclarations)
    {
        // TODO error for duplicates

        var resolver = new TypeResolver(syn.File, diagnostics, typeDeclarations);
        if (syn is IClassDeclarationSyntax { BaseTypeName: not null and var baseTypeName })
        {
            var baseType = resolver.EvaluateBareType(baseTypeName);
            if (baseType is ObjectType { DeclaredType: var declaredType })
            {
                if (!declaredType.IsClass)
                    diagnostics.Add(TypeError.BaseTypeMustBeClass(syn.File, syn.Name, baseTypeName));

                yield return declaredType;
            }
            else
                diagnostics.Add(TypeError.BaseTypeMustBeClass(syn.File, syn.Name, baseTypeName));
        }

        foreach (var superTypeSyntax in syn.SupertypeNames)
        {
            var superType = resolver.EvaluateBareType(superTypeSyntax);
            if (superType is ObjectType { DeclaredType: var declaredType })
                yield return declaredType;
            else
                diagnostics.Add(TypeError.SuperTypeMustBeClassOrTrait(syn.File, syn.Name, superTypeSyntax));
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
                        types.Add(new ParameterType(false, fieldSymbol.DataType));
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
                        diagnostics.Add(TypeError.LentIdentity(file, namedParam.Span));
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

    private static ObjectType ResolveConstructorSelfParameterType(
        TypeResolver resolver,
        ISelfParameterSyntax selfParameter,
        IClassDeclarationSyntax declaringClass)
    {
        var declaredType = declaringClass.Symbol.Result.DeclaresType;
        return resolver.EvaluateConstructorSelfParameterType(declaredType, selfParameter.Capability, declaredType.GenericParameterDataTypes);
    }

    private static ParameterType ResolveMethodSelfParameterType(
        TypeResolver resolver,
        ISelfParameterSyntax selfParameter,
        ITypeDeclarationSyntax declaringType)
    {
        var declaredType = declaringType.Symbol.Result.DeclaresType;
        var selfType = resolver.EvaluateMethodSelfParameterType(declaredType, selfParameter.Capability, declaredType.GenericParameterDataTypes);
        return new ParameterType(selfParameter.IsLentBinding, selfType);
    }

    private void BuildSelfParameterSymbol(
        InvocableSymbol containingSymbol,
        CodeFile file,
        ISelfParameterSyntax param,
        DataType type,
        bool isConstructor = false)
    {
        if (isConstructor && param.IsLentBinding)
            diagnostics.Add(SemanticError.LentConstructorSelf(file, param.Span));
        var symbol = new SelfParameterSymbol(containingSymbol, param.IsLentBinding && !isConstructor, type);
        param.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
    }

    private ReturnType ResolveReturnType(
        CodeFile file,
        IReturnSyntax? returnSyntax,
        TypeResolver resolver)
    {
        if (returnSyntax is null)
            return ReturnType.Void;
        DataType type = resolver.Evaluate(returnSyntax.Type);
        var isLent = returnSyntax.IsLent;
        if (isLent && type is ReferenceType { IsIdentityReference: true })
        {
            diagnostics.Add(TypeError.LentIdentity(file, returnSyntax.Span));
            isLent = false;
        }
        return new ReturnType(isLent, type);
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

        public ITypeDeclarationSyntax this[IPromise<TypeSymbol> symbol] => typeDeclarations[symbol];

        #region IEnumerable<ITypeDeclarationSyntax>
        public IEnumerator<ITypeDeclarationSyntax> GetEnumerator() => typeDeclarations.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
