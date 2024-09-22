using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public sealed class ContextualizedOverload
{
    public static ContextualizedOverload Create(IFunctionInvocableDeclarationNode function)
        => new(null, function.ParameterTypes, function.ReturnType);

    public static ContextualizedOverload Create(
        IMaybeExpressionType constructingType,
        IConstructorDeclarationNode constructor)
        => Create(constructingType, constructor, constructor.SelfParameterType);

    public static ContextualizedOverload Create(
        IMaybeExpressionType initializingAntetype,
        IInitializerDeclarationNode initializer)
        => Create(initializingAntetype, initializer, initializer.SelfParameterType);

    public static ContextualizedOverload Create(
        IMaybeExpressionType contextType,
        IStandardMethodDeclarationNode method)
        => Create(contextType, method, method.SelfParameterType);

    public static ContextualizedOverload Create<T>(
        IMaybeExpressionType contextType,
        T propertyAccessor)
        where T : IPropertyAccessorDeclarationNode
        => Create(contextType, propertyAccessor, propertyAccessor.SelfParameterType);

    public static ContextualizedOverload Create(FunctionType functionType)
        => new(null, functionType.Parameters, functionType.Return);

    private static ContextualizedOverload Create(
        IMaybeExpressionType contextType,
        IInvocableDeclarationNode node,
        SelfParameterType selfParameterType)
    {
        if (contextType is NonEmptyType nonEmptyContextType)
        {
            selfParameterType = CreateSelfParameterType(nonEmptyContextType, selfParameterType);
            var parameterTypes = CreateParameterTypes(nonEmptyContextType, node);
            var returnType = CreateReturnType(nonEmptyContextType, node);
            return new(selfParameterType, parameterTypes, returnType);
        }
        return new(selfParameterType, node.ParameterTypes, node.ReturnType);
    }

    private static SelfParameterType CreateSelfParameterType(
        NonEmptyType contextType,
        SelfParameterType symbolSelfParameterType)
        => contextType.ReplaceTypeParametersIn(symbolSelfParameterType);

    private static IFixedList<ParameterType> CreateParameterTypes(
        NonEmptyType contextType,
        IInvocableDeclarationNode node)
        => node.ParameterTypes.Select(p => CreateParameterType(contextType, p))
               .Where(p => p is not { Type: VoidType }).ToFixedList();

    private static ParameterType CreateParameterType(NonEmptyType contextType, ParameterType parameter)
        => contextType.ReplaceTypeParametersIn(parameter);

    private static IMaybeType CreateReturnType(NonEmptyType contextType, IInvocableDeclarationNode node)
        => contextType.ReplaceTypeParametersIn(node.ReturnType);

    public SelfParameterType? SelfParameterType { get; }
    public IFixedList<ParameterType> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public IMaybeExpressionType ReturnType { get; }

    private ContextualizedOverload(
        SelfParameterType? selfParameterType,
        IEnumerable<ParameterType> parameterTypes,
        IMaybeExpressionType returnType)
    {
        SelfParameterType = selfParameterType;
        ParameterTypes = parameterTypes.ToFixedList();
        ReturnType = returnType;
    }
}
