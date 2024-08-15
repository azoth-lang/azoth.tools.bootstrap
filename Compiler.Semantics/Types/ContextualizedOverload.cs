using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public sealed class ContextualizedOverload
{
    public static ContextualizedOverload Create(
        IFunctionLikeDeclarationNode function)
    {
        var symbol = function.Symbol;
        var parameterTypes = symbol.Parameters;
        var returnType = symbol.Return;
        return new(null, parameterTypes, returnType);
    }

    public static ContextualizedOverload Create(
        DataType constructingType,
        IConstructorDeclarationNode constructor)
    {
        var symbol = constructor.Symbol;
        return Create(constructingType, symbol, new(false, symbol.SelfParameterType));
    }

    public static ContextualizedOverload Create(
        DataType initializingAntetype, IInitializerDeclarationNode initializer)
    {
        var symbol = initializer.Symbol;
        return Create(initializingAntetype, symbol, new(false, symbol.SelfParameterType));
    }

    public static ContextualizedOverload Create(
        DataType contextType,
        IStandardMethodDeclarationNode method)
    {
        var symbol = method.Symbol;
        return Create(contextType, symbol, symbol.SelfParameterType);
    }

    public static ContextualizedOverload Create<T>(
        DataType contextType,
        T propertyAccessor)
        where T : IPropertyAccessorDeclarationNode
    {
        var symbol = propertyAccessor.Symbol;
        return Create(contextType, symbol, symbol.SelfParameterType);
    }

    public static ContextualizedOverload Create(FunctionType functionType)
        => new(null, functionType.Parameters, functionType.Return);

    private static ContextualizedOverload Create(
        DataType contextType,
        InvocableSymbol symbol,
        SelfParameterType selfParameterType)
    {
        if (contextType is NonEmptyType nonEmptyContextType)
        {
            selfParameterType = CreateSelfParameterType(nonEmptyContextType, selfParameterType);
            var parameterTypes = CreateParameterTypes(nonEmptyContextType, symbol);
            var returnType = CreateReturnType(nonEmptyContextType, symbol);
            return new(selfParameterType, parameterTypes, returnType);
        }
        return new(selfParameterType, symbol.Parameters, symbol.Return);
    }

    private static SelfParameterType CreateSelfParameterType(
        NonEmptyType contextType,
        SelfParameterType symbolSelfParameterType)
        => contextType.ReplaceTypeParametersIn(symbolSelfParameterType);

    private static IFixedList<ParameterType> CreateParameterTypes(
        NonEmptyType contextType,
        InvocableSymbol symbol)
        => symbol.Parameters.Select(p => CreateParameterType(contextType, p))
                 .Where(p => p is not { Type: VoidType }).ToFixedList();

    private static ParameterType CreateParameterType(NonEmptyType contextType, ParameterType parameter)
        => contextType.ReplaceTypeParametersIn(parameter);

    private static ReturnType CreateReturnType(NonEmptyType contextType, InvocableSymbol symbol)
        => contextType.ReplaceTypeParametersIn(symbol.Return);

    public SelfParameterType? SelfParameterType { get; }
    public IFixedList<ParameterType> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public ReturnType ReturnType { get; }

    private ContextualizedOverload(
        SelfParameterType? selfParameterType,
        IEnumerable<ParameterType> parameterTypes,
        ReturnType returnType)
    {
        SelfParameterType = selfParameterType;
        ParameterTypes = parameterTypes.ToFixedList();
        ReturnType = returnType;
    }
}
