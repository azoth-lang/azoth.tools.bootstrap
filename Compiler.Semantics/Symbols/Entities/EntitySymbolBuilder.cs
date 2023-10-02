using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
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
        // Process all classes first because they may be referenced by functions etc.
        foreach (var @class in entities.OfType<IClassDeclarationSyntax>())
            BuildClassSymbol(@class);

        // Now resolve all other symbols (class declarations will already have symbols and won't be processed again)
        foreach (var entity in entities)
            BuildEntitySymbol(entity);
    }

    /// <summary>
    /// If the type has not been resolved, this resolves it. This function
    /// also watches for type cycles and reports an error.
    /// </summary>
    private void BuildEntitySymbol(IEntityDeclarationSyntax entity)
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
            case IClassDeclarationSyntax syn:
                BuildClassSymbol(syn);
                break;
        }
    }

    private void BuildMethodSymbol(IMethodDeclarationSyntax method)
    {
        method.Symbol.BeginFulfilling();
        var declaringClassSymbol = method.DeclaringClass.Symbol.Result;
        var resolver = new TypeResolver(method.File, diagnostics);
        var selfParameterType = ResolveMethodSelfParameterType(resolver, method.SelfParameter, method.DeclaringClass);
        var parameterTypes = ResolveParameterTypes(resolver, method.Parameters, method.DeclaringClass);
        var returnType = ResolveReturnType(method.Return, resolver);
        var symbol = new MethodSymbol(declaringClassSymbol, method.Name, selfParameterType, parameterTypes, returnType);
        method.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildSelfParameterSymbol(symbol, method.SelfParameter, selfParameterType.Type);
        BuildParameterSymbols(symbol, method.Parameters, parameterTypes);
    }

    private void BuildConstructorSymbol(IConstructorDeclarationSyntax constructor)
    {
        constructor.Symbol.BeginFulfilling();
        var resolver = new TypeResolver(constructor.File, diagnostics);
        var selfParameterType = ResolveConstructorSelfParameterType(resolver, constructor.SelfParameter, constructor.DeclaringClass);
        var parameterTypes = ResolveParameterTypes(resolver, constructor.Parameters, constructor.DeclaringClass);

        var declaringClassSymbol = constructor.DeclaringClass.Symbol.Result;
        var symbol = new ConstructorSymbol(declaringClassSymbol, constructor.Name, selfParameterType, parameterTypes);
        constructor.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildSelfParameterSymbol(symbol, constructor.SelfParameter, selfParameterType);
        BuildParameterSymbols(symbol, constructor.Parameters, parameterTypes);
    }

    private void BuildAssociatedFunctionSymbol(IAssociatedFunctionDeclarationSyntax associatedFunction)
    {
        associatedFunction.Symbol.BeginFulfilling();
        var resolver = new TypeResolver(associatedFunction.File, diagnostics);
        var parameterTypes = ResolveParameterTypes(resolver, associatedFunction.Parameters);
        var returnType = ResolveReturnType(associatedFunction.Return, resolver);
        var declaringClassSymbol = associatedFunction.DeclaringClass.Symbol.Result;
        var symbol = new FunctionSymbol(declaringClassSymbol, associatedFunction.Name, parameterTypes, returnType);
        associatedFunction.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildParameterSymbols(symbol, associatedFunction.Parameters, parameterTypes);
    }

    private FieldSymbol BuildFieldSymbol(IFieldDeclarationSyntax field)
    {
        if (field.Symbol.IsFulfilled)
            return field.Symbol.Result;

        field.Symbol.BeginFulfilling();
        var resolver = new TypeResolver(field.File, diagnostics);
        var type = resolver.Evaluate(field.Type);
        var symbol = new FieldSymbol(field.DeclaringClass.Symbol.Result, field.Name, field.IsMutableBinding, type);
        field.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        return symbol;
    }

    private void BuildFunctionSymbol(IFunctionDeclarationSyntax function)
    {
        function.Symbol.BeginFulfilling();
        var resolver = new TypeResolver(function.File, diagnostics);
        var parameterTypes = ResolveParameterTypes(resolver, function.Parameters);
        var returnType = ResolveReturnType(function.Return, resolver);
        var symbol = new FunctionSymbol(function.ContainingNamespaceSymbol, function.Name, parameterTypes, returnType);
        function.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
        BuildParameterSymbols(symbol, function.Parameters, parameterTypes);
    }

    private void BuildClassSymbol(IClassDeclarationSyntax @class)
    {
        if (!@class.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

        var typeParameters = @class.GenericParameters.Select(p => new GenericParameter(p.Name)).ToFixedList();
        var packageName = @class.ContainingNamespaceSymbol.Package!.Name;

        var superTypes = FixedSet<DeclaredObjectType>.Empty;
        if (@class.BaseType is not null)
        {
            var resolver = new TypeResolver(@class.File, diagnostics);
            var baseType = resolver.EvaluateBareType(@class.BaseType);
            if (baseType is ObjectType { DeclaredType: var declaredType })
                superTypes = declaredType.Yield().ToFixedSet();
            else
                diagnostics.Add(TypeError.BaseTypeMustBeClass(@class.File, @class.Name, @class.BaseType));
        }

        var classType = DeclaredObjectType.Create(packageName, @class.ContainingNamespaceName,
            @class.Name, @class.IsConst, typeParameters, superTypes);

        var classSymbol = new ObjectTypeSymbol(@class.ContainingNamespaceSymbol, classType);
        @class.Symbol.Fulfill(classSymbol);
        symbolTree.Add(classSymbol);

        // TODO the generic parameter symbols are needed while evaluating base types
        BuildGenericParameterSymbols(@class, classSymbol);
        @class.CreateDefaultConstructor(symbolTree);
        return;

        void AddCircularDefinitionError()
        {
            diagnostics.Add(TypeError.CircularDefinition(@class.File, @class.NameSpan, @class));
        }
    }

    private void BuildGenericParameterSymbols(IClassDeclarationSyntax @class, ObjectTypeSymbol classSymbol)
    {
        var classType = classSymbol.DeclaresType;
        foreach (var (genericParameter, type) in @class.GenericParameters.Zip(classType.GenericParameterTypes))
        {
            var genericParameterSymbol = new GenericParameterTypeSymbol(classSymbol, type);
            genericParameter.Symbol.Fulfill(genericParameterSymbol);
            symbolTree.Add(genericParameterSymbol);
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
        IClassDeclarationSyntax declaringClass)
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
                    var field = declaringClass.Members.OfType<IFieldDeclarationSyntax>()
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
        IEnumerable<IConstructorParameterSyntax> parameters,
        IEnumerable<ParameterType> parameterTypes)
        => BuildParameterSymbols(containingSymbol, parameters, parameterTypes.Select(pt => pt.Type));

    private void BuildParameterSymbols(
        InvocableSymbol containingSymbol,
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
                    var symbol = VariableSymbol.CreateParameter(containingSymbol, namedParam.Name,
                        namedParam.DeclarationNumber.Result, namedParam.IsMutableBinding, namedParam.IsLentBinding, type);
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
        IClassDeclarationSyntax declaringClass)
    {
        var declaredType = declaringClass.Symbol.Result.DeclaresType;
        var selfType = resolver.EvaluateMethodSelfParameterType(declaredType, selfParameter.Capability, declaredType.GenericParameterDataTypes);
        return new ParameterType(selfParameter.IsLentBinding, selfType);
    }

    private void BuildSelfParameterSymbol(
        InvocableSymbol containingSymbol,
        ISelfParameterSyntax param,
        DataType type)
    {
        var symbol = new SelfParameterSymbol(containingSymbol, param.IsLentBinding, type);
        param.Symbol.Fulfill(symbol);
        symbolTree.Add(symbol);
    }

    private static ReturnType ResolveReturnType(
        IReturnSyntax? returnSyntax, TypeResolver resolver)
    {
        if (returnSyntax is null)
            return ReturnType.Void;
        DataType type = resolver.Evaluate(returnSyntax.Type);
        return new ReturnType(returnSyntax.IsLent, type);
    }
}
