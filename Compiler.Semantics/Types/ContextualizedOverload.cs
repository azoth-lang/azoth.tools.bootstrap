using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public sealed class ContextualizedOverload<TDeclaration> : ContextualizedOverload
    where TDeclaration : IInvocableDeclarationNode
{
    public TDeclaration Declaration { get; }

    internal ContextualizedOverload(
        TDeclaration declaration,
        SelfParameterType? selfParameterType,
        IEnumerable<ParameterType> parameterTypes,
        ReturnType returnType)
        : base(selfParameterType, parameterTypes, returnType)
    {
        Declaration = declaration;
    }

    //public bool CompatibleWith(ArgumentAntetypes arguments)
    //{
    //    if (Arity != arguments.Arity)
    //        return false;

    //    if (SelfParameterType is not null
    //        // Self is null for constructors and initializers where the type is definitely compatible
    //        && arguments.Self is not null)
    //    {
    //        if (!arguments.Self.IsAssignableTo(SelfParameterType))
    //            return false;
    //    }

    //    return ParameterTypes.EquiZip(arguments.Arguments).All((p, a) => a.IsAssignableTo(p));
    //}
}

public abstract class ContextualizedOverload
{
    public static ContextualizedOverload<IFunctionLikeDeclarationNode> Create(
        IFunctionLikeDeclarationNode function)
    {
        var symbol = function.Symbol;
        var parameterTypes = symbol.Parameters;
        var returnAntetype = symbol.Return;
        return new(function, null, parameterTypes, returnAntetype);
    }

    public static ContextualizedOverload<IConstructorDeclarationNode> Create(
        DataType constructingType,
        IConstructorDeclarationNode constructor)
    {
        var symbol = constructor.Symbol;
        return Create(constructingType, constructor, symbol, new(false, symbol.SelfParameterType));
    }

    public static ContextualizedOverload<IInitializerDeclarationNode> Create(
        DataType initializingAntetype, IInitializerDeclarationNode initializer)
    {
        var symbol = initializer.Symbol;
        return Create(initializingAntetype, initializer, symbol, new(false, symbol.SelfParameterType));
    }

    public static ContextualizedOverload<IStandardMethodDeclarationNode> Create(
        DataType contextType,
        IStandardMethodDeclarationNode method)
    {
        var symbol = method.Symbol;
        return Create(contextType, method, symbol, symbol.SelfParameterType);
    }

    private static ContextualizedOverload<TDeclaration> Create<TDeclaration>(
        DataType contextType,
        TDeclaration declaration,
        InvocableSymbol symbol,
        SelfParameterType selfParameterType)
        where TDeclaration : IInvocableDeclarationNode
    {
        if (contextType is NonEmptyType nonEmptyContextType)
        {
            selfParameterType = CreateSelfParameterType(nonEmptyContextType, selfParameterType);
            var parameterTypes = CreateParameterTypes(nonEmptyContextType, symbol);
            var returnType = CreateReturnType(nonEmptyContextType, symbol);
            return new(declaration, selfParameterType, parameterTypes, returnType);
        }
        return new(declaration, selfParameterType, symbol.Parameters, symbol.Return);
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

    private protected ContextualizedOverload(
        SelfParameterType? selfParameterType,
        IEnumerable<ParameterType> parameterTypes,
        ReturnType returnType)
    {
        SelfParameterType = selfParameterType;
        ParameterTypes = parameterTypes.ToFixedList();
        ReturnType = returnType;
    }
}
