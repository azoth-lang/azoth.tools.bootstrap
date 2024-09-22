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
        IMaybeSelfParameterType selfParameterType)
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

    private static IMaybeSelfParameterType CreateSelfParameterType(
        NonEmptyType contextType,
        IMaybeSelfParameterType symbolSelfParameterType)
        => contextType.ReplaceTypeParametersIn(symbolSelfParameterType);

    private static IFixedList<IMaybeParameterType> CreateParameterTypes(
        NonEmptyType contextType,
        IInvocableDeclarationNode node)
        => node.ParameterTypes.Select(p => CreateParameterType(contextType, p)).WhereNotNull().ToFixedList();

    private static IMaybeParameterType? CreateParameterType(NonEmptyType contextType, IMaybeParameterType parameter)
        => contextType.ReplaceTypeParametersIn(parameter);

    private static IMaybeType CreateReturnType(NonEmptyType contextType, IInvocableDeclarationNode node)
        => contextType.ReplaceTypeParametersIn(node.ReturnType);

    public IMaybeSelfParameterType? SelfParameterType { get; }
    public IFixedList<IMaybeParameterType> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public IMaybeType ReturnType { get; }

    private ContextualizedOverload(
        IMaybeSelfParameterType? selfParameterType,
        IFixedList<IMaybeParameterType> parameterTypes,
        IMaybeType returnType)
    {
        SelfParameterType = selfParameterType;
        ParameterTypes = parameterTypes.ToFixedList();
        ReturnType = returnType;
    }
}
