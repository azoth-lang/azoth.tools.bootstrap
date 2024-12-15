using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public sealed class ContextualizedCall : IEquatable<ContextualizedCall>
{
    public static ContextualizedCall Create(IFunctionInvocableDeclarationNode function)
        => new(null, null, function.ParameterTypes, function.ReturnType);

    public static ContextualizedCall Create(
        IMaybeType constructingType,
        IConstructorDeclarationNode constructor)
        => Create(constructingType, constructor, constructor.SelfParameterType);

    public static ContextualizedCall Create(
        IMaybeType initializingType,
        IInitializerDeclarationNode initializer)
        => Create(initializingType, initializer, initializer.SelfParameterType);

    public static ContextualizedCall Create(
        IMaybeType contextType,
        IOrdinaryMethodDeclarationNode method)
        => Create(contextType, method, method.SelfParameterType);

    public static ContextualizedCall Create<T>(
        IMaybeType contextType,
        T propertyAccessor)
        where T : IPropertyAccessorDeclarationNode
        => Create(contextType, propertyAccessor, propertyAccessor.SelfParameterType);

    public static ContextualizedCall Create(FunctionType functionType)
        => new(null, null, functionType.Parameters, functionType.Return);

    private static ContextualizedCall Create(
        IMaybeType contextType,
        IInvocableDeclarationNode node,
        IMaybeNonVoidType selfParameterType)
    {
        if (contextType is NonVoidType nonVoidContextType)
        {
            selfParameterType = CreateSelfParameterType(nonVoidContextType, selfParameterType);
            var parameterTypes = CreateParameterTypes(nonVoidContextType, node);
            var returnType = CreateReturnType(nonVoidContextType, node);
            return new(contextType, selfParameterType, parameterTypes, returnType);
        }
        return new(contextType, selfParameterType, node.ParameterTypes, node.ReturnType);
    }

    private static IMaybeNonVoidType CreateSelfParameterType(
        NonVoidType contextType,
        IMaybeNonVoidType symbolSelfParameterType)
        // TODO eliminate cast
        => (IMaybeNonVoidType)contextType.TypeReplacements.ReplaceTypeParametersIn(symbolSelfParameterType);

    private static IFixedList<IMaybeParameterType> CreateParameterTypes(
        NonVoidType contextType,
        IInvocableDeclarationNode node)
        => node.ParameterTypes.Select(p => CreateParameterType(contextType, p)).WhereNotNull().ToFixedList();

    private static IMaybeParameterType? CreateParameterType(NonVoidType contextType, IMaybeParameterType parameter)
        => contextType.TypeReplacements.ReplaceTypeParametersIn(parameter);

    private static IMaybeType CreateReturnType(NonVoidType contextType, IInvocableDeclarationNode node)
        => contextType.TypeReplacements.ReplaceTypeParametersIn(node.ReturnType);

    public IMaybeType? ContextType { get; }
    public IMaybeNonVoidType? SelfParameterType { get; }
    public IFixedList<IMaybeParameterType> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public IMaybeType ReturnType { get; }

    private ContextualizedCall(
        IMaybeType? contextType,
        IMaybeNonVoidType? selfParameterType,
        IFixedList<IMaybeParameterType> parameterTypes,
        IMaybeType returnType)
    {
        ContextType = contextType;
        SelfParameterType = selfParameterType;
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }

    #region Equality
    public bool Equals(ContextualizedCall? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Equals(ContextType, other.ContextType)
               && Equals(SelfParameterType, other.SelfParameterType)
               && ParameterTypes.Equals(other.ParameterTypes)
               && ReturnType.Equals(other.ReturnType);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is ContextualizedCall other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(ContextType, SelfParameterType, ParameterTypes, ReturnType);
    #endregion
}
