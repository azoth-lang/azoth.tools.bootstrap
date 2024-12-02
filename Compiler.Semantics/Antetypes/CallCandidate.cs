using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

public sealed class CallCandidate<TDeclaration>
    where TDeclaration : IInvocableDeclarationNode
{
    public TDeclaration Declaration { get; }
    public IMaybePlainType? SelfParameterPlainType { get; }
    public IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes { get; }
    public int Arity => ParameterPlainTypes.Count;
    public IMaybePlainType ReturnPlainType { get; }

    internal CallCandidate(
        TDeclaration declaration,
        IMaybePlainType? selfParameterPlainType,
        IEnumerable<IMaybeNonVoidPlainType> parameterPlainTypes,
        IMaybePlainType returnPlainType)
    {
        SelfParameterPlainType = selfParameterPlainType;
        ParameterPlainTypes = parameterPlainTypes.ToFixedList();
        ReturnPlainType = returnPlainType;
        Declaration = declaration;
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
}

internal static class CallCandidate
{
    public static CallCandidate<IFunctionInvocableDeclarationNode> Create(
        IFunctionInvocableDeclarationNode function)
    {
        var parameterAntetypes = function.ParameterTypes.Select(p => p.Type.ToPlainType().ToNonLiteralType())
                                       .Cast<IMaybeNonVoidPlainType>().ToFixedList();
        var returnAntetype = function.ReturnType.ToPlainType();
        return new(function, null, parameterAntetypes, returnAntetype);
    }

    public static CallCandidate<IConstructorDeclarationNode> Create(
        IMaybePlainType constructingPlainType,
        IConstructorDeclarationNode constructor)
        => Create(constructingPlainType, constructor, constructor.SelfParameterType);

    public static CallCandidate<IInitializerDeclarationNode> Create(
        IMaybePlainType initializingPlainType,
        IInitializerDeclarationNode initializer)
        => Create(initializingPlainType, initializer, initializer.SelfParameterType);

    public static CallCandidate<IStandardMethodDeclarationNode> Create(
        IMaybePlainType contextPlainType,
        IStandardMethodDeclarationNode method)
        => Create(contextPlainType, method, method.SelfParameterType);

    private static CallCandidate<TDeclaration> Create<TDeclaration>(
        IMaybePlainType contextPlainType,
        TDeclaration declaration,
        IMaybeSelfParameterType selfParameterType)
        where TDeclaration : IInvocableDeclarationNode
    {
        var selfParameterAntetype = SelfParameterPlainType(contextPlainType, selfParameterType);
        var parameterAntetypes = ParameterPlainTypes(contextPlainType, declaration);
        var returnAntetype = ReturnPlainType(contextPlainType, declaration);
        return new(declaration, selfParameterAntetype, parameterAntetypes, returnAntetype);
    }

    private static IMaybePlainType SelfParameterPlainType(
        IMaybePlainType contextPlainType,
        IMaybeSelfParameterType selfParameterType)
        => contextPlainType.ReplaceTypeParametersIn(selfParameterType.Type.ToPlainType())
                               .ToNonLiteralType();

    private static IFixedList<IMaybeNonVoidPlainType> ParameterPlainTypes(
        IMaybePlainType contextPlainType,
        IInvocableDeclarationNode declaration)
        => declaration.ParameterTypes.Select(p => ParameterPlainType(contextPlainType, p))
                      .OfType<IMaybeNonVoidPlainType>().ToFixedList();

    private static IMaybePlainType ParameterPlainType(IMaybePlainType contextPlainType, IMaybeParameterType parameter)
        => contextPlainType.ReplaceTypeParametersIn(parameter.Type.ToPlainType()).ToNonLiteralType();

    private static IMaybePlainType ReturnPlainType(IMaybePlainType contextPlainType, IInvocableDeclarationNode declaration)
        => contextPlainType.ReplaceTypeParametersIn(declaration.ReturnType.ToPlainType()).ToNonLiteralType();
}
