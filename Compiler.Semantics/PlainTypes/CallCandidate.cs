using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

public sealed class CallCandidate<TDeclaration> : IEquatable<CallCandidate<TDeclaration>>
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
    public bool Equals(CallCandidate<TDeclaration>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(ContextPlainType, other.ContextPlainType)
               && Declaration.Equals(other.Declaration)
               && Equals(SelfParameterPlainType, other.SelfParameterPlainType)
               && ParameterPlainTypes.Equals(other.ParameterPlainTypes)
               && ReturnPlainType.Equals(other.ReturnPlainType);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is CallCandidate<TDeclaration> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(ContextPlainType, Declaration, SelfParameterPlainType, ParameterPlainTypes, ReturnPlainType);
    #endregion
}

internal static class CallCandidate
{
    public static CallCandidate<IFunctionInvocableDeclarationNode> Create(
        IFunctionInvocableDeclarationNode function)
    {
        var parameterPlainTypes = function.ParameterPlainTypes;
        var returnPlainType = function.ReturnPlainType;
        return new(null, function, null, parameterPlainTypes, returnPlainType);
    }

    public static CallCandidate<IConstructorDeclarationNode> Create(
        IMaybePlainType constructingPlainType,
        IConstructorDeclarationNode constructor)
        => Create(constructingPlainType, constructor, constructor.SelfParameterPlainType);

    public static CallCandidate<IInitializerDeclarationNode> Create(
        IMaybePlainType initializingPlainType,
        IInitializerDeclarationNode initializer)
        => Create(initializingPlainType, initializer, initializer.SelfParameterPlainType);

    public static CallCandidate<IOrdinaryMethodDeclarationNode> Create(
        IMaybePlainType contextPlainType,
        IOrdinaryMethodDeclarationNode method)
        => Create(contextPlainType, method, method.SelfParameterPlainType);

    private static CallCandidate<TDeclaration> Create<TDeclaration>(
        IMaybePlainType contextPlainType,
        TDeclaration declaration,
        IMaybeNonVoidPlainType selfParameterPlainType)
        where TDeclaration : IInvocableDeclarationNode
    {
        selfParameterPlainType = SelfParameterPlainType(contextPlainType, selfParameterPlainType);
        var parameterPlainTypes = ParameterPlainTypes(contextPlainType, declaration);
        var returnPlainType = ReturnPlainType(contextPlainType, declaration);
        return new(contextPlainType, declaration, selfParameterPlainType, parameterPlainTypes, returnPlainType);
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
