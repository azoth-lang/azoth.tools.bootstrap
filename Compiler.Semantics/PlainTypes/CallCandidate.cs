using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

public interface ICallCandidate<out TDeclaration> : IEquatable<ICallCandidate<IInvocableDeclarationNode>>
    where TDeclaration : IInvocableDeclarationNode
{
    IMaybePlainType? ContextPlainType { get; }
    TDeclaration Declaration { get; }
    IMaybeNonVoidPlainType? SelfParameterPlainType { get; }
    IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes { get; }
    int Arity { get; }
    IMaybePlainType ReturnPlainType { get; }

    bool CompatibleWith(ArgumentPlainTypes arguments);
}

internal sealed class CallCandidate<TDeclaration> : ICallCandidate<TDeclaration>
    where TDeclaration : IInvocableDeclarationNode
{
    public IMaybePlainType? ContextPlainType { get; }
    public TDeclaration Declaration { get; }
    public IMaybeNonVoidPlainType? SelfParameterPlainType { get; }
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes { get; }
    public int Arity => ParameterPlainTypes.Count;
    public IMaybePlainType ReturnPlainType { get; }

    internal CallCandidate(
        IMaybePlainType? contextPlainType,
        TDeclaration declaration,
        IMaybeNonVoidPlainType? selfParameterPlainType,
        IEnumerable<IMaybeNonVoidPlainType> parameterPlainTypes,
        IMaybePlainType returnPlainType)
    {
        ContextPlainType = contextPlainType;
        Declaration = declaration;
        SelfParameterPlainType = selfParameterPlainType;
        ParameterPlainTypes = parameterPlainTypes.ToFixedList();
        ReturnPlainType = returnPlainType;
    }

    public bool CompatibleWith(ArgumentPlainTypes arguments)
    {
        if (Arity != arguments.Arity)
            return false;

        if (SelfParameterPlainType is not null
            // Self is null for constructors and initializers where the type is definitely compatible
            && arguments.Self is not null)
        {
            if (!arguments.Self.IsAssignableTo(SelfParameterPlainType))
                return false;
        }

        return ParameterPlainTypes.EquiZip(arguments.Arguments).All((p, a) => a.IsAssignableTo(p));
    }

    #region Equality
    public bool Equals(ICallCandidate<IInvocableDeclarationNode>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CallCandidate<TDeclaration> o
               && Equals(ContextPlainType, o.ContextPlainType)
               && Declaration.Equals(o.Declaration)
               && Equals(SelfParameterPlainType, o.SelfParameterPlainType)
               && ParameterPlainTypes.Equals(o.ParameterPlainTypes)
               && ReturnPlainType.Equals(o.ReturnPlainType);
    }

    public override bool Equals(object? obj)
        // For some reason, checking for `is CallCandidate<TDeclaration>` makes Equals a recursive
        // call. So instead have to only check for `ICallCandidate<IInvocableDeclarationNode>`.
        => ReferenceEquals(this, obj) || obj is ICallCandidate<IInvocableDeclarationNode> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(ContextPlainType, Declaration, SelfParameterPlainType, ParameterPlainTypes, ReturnPlainType);
    #endregion
}

internal static class CallCandidate
{
    public static ICallCandidate<IFunctionInvocableDeclarationNode> Create(
        IFunctionInvocableDeclarationNode function)
    {
        var parameterPlainTypes = function.ParameterPlainTypes;
        var returnPlainType = function.ReturnPlainType;
        return new CallCandidate<IFunctionInvocableDeclarationNode>(null, function, null, parameterPlainTypes, returnPlainType);
    }

    public static ICallCandidate<IConstructorDeclarationNode> Create(
        IMaybePlainType constructingPlainType,
        IConstructorDeclarationNode constructor)
        => Create(constructingPlainType, constructor, constructor.SelfParameterPlainType);

    public static ICallCandidate<IInitializerDeclarationNode> Create(
        IMaybePlainType initializingPlainType,
        IInitializerDeclarationNode initializer)
        => Create(initializingPlainType, initializer, initializer.SelfParameterPlainType);

    public static ICallCandidate<IOrdinaryMethodDeclarationNode> Create(
        IMaybePlainType contextPlainType,
        IOrdinaryMethodDeclarationNode method)
        => Create(contextPlainType, method, method.SelfParameterPlainType);

    public static ICallCandidate<IPropertyAccessorDeclarationNode> Create(
        IMaybePlainType contextPlainType,
        IPropertyAccessorDeclarationNode property)
        => property switch
        {
            IGetterMethodDeclarationNode node
                => Create(contextPlainType, node, property.SelfParameterPlainType),
            ISetterMethodDeclarationNode node
                => Create(contextPlainType, node, property.SelfParameterPlainType),
            _ => throw ExhaustiveMatch.Failed(property),
        };

    private static ICallCandidate<TDeclaration> Create<TDeclaration>(
        IMaybePlainType contextPlainType,
        TDeclaration declaration,
        IMaybeNonVoidPlainType selfParameterPlainType)
        where TDeclaration : IInvocableDeclarationNode
    {
        selfParameterPlainType = SelfParameterPlainType(contextPlainType, selfParameterPlainType);
        var parameterPlainTypes = ParameterPlainTypes(contextPlainType, declaration);
        var returnPlainType = ReturnPlainType(contextPlainType, declaration);
        return new CallCandidate<TDeclaration>(contextPlainType, declaration, selfParameterPlainType, parameterPlainTypes, returnPlainType);
    }

    private static IMaybeNonVoidPlainType SelfParameterPlainType(
        IMaybePlainType contextPlainType,
        IMaybeNonVoidPlainType selfParameterPlainType)
        => contextPlainType.TypeReplacements.Apply(selfParameterPlainType).ToNonVoid();

    private static IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes(
        IMaybePlainType contextPlainType,
        IInvocableDeclarationNode declaration)
        => declaration.ParameterPlainTypes.Select(p => ParameterPlainType(contextPlainType, p))
                      .OfType<IMaybeNonVoidPlainType>().ToFixedList();

    private static IMaybePlainType ParameterPlainType(IMaybePlainType contextPlainType, IMaybeNonVoidPlainType parameterPlainType)
        => contextPlainType.TypeReplacements.Apply(parameterPlainType);

    private static IMaybePlainType ReturnPlainType(IMaybePlainType contextPlainType, IInvocableDeclarationNode declaration)
        => contextPlainType.TypeReplacements.Apply(declaration.ReturnPlainType);
}
