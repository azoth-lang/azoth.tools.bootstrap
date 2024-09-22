using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal sealed class AntetypeContextualizedOverload<TDeclaration>
    where TDeclaration : IInvocableDeclarationNode
{
    public TDeclaration Declaration { get; }
    public IMaybeAntetype? SelfParameterAntetype { get; }
    public IFixedList<IMaybeNonVoidAntetype> ParameterAntetypes { get; }
    public int Arity => ParameterAntetypes.Count;
    public IMaybeAntetype ReturnAntetype { get; }

    internal AntetypeContextualizedOverload(
        TDeclaration declaration,
        IMaybeAntetype? selfParameterAntetype,
        IEnumerable<IMaybeNonVoidAntetype> parameterAntetypes,
        IMaybeAntetype returnAntetype)
    {
        SelfParameterAntetype = selfParameterAntetype;
        ParameterAntetypes = parameterAntetypes.ToFixedList();
        ReturnAntetype = returnAntetype;
        Declaration = declaration;
    }

    public bool CompatibleWith(ArgumentAntetypes arguments)
    {
        if (Arity != arguments.Arity)
            return false;

        if (SelfParameterAntetype is not null
            // Self is null for constructors and initializers where the type is definitely compatible
            && arguments.Self is not null)
        {
            if (!arguments.Self.IsAssignableTo(SelfParameterAntetype))
                return false;
        }

        return ParameterAntetypes.EquiZip(arguments.Arguments).All((p, a) => a.IsAssignableTo(p));
    }
}

internal static class AntetypeContextualizedOverload
{
    public static AntetypeContextualizedOverload<IFunctionInvocableDeclarationNode> Create(
        IFunctionInvocableDeclarationNode function)
    {
        var parameterAntetypes = function.ParameterTypes.Select(p => p.Type.ToAntetype().ToNonConstValueType())
                                       .Cast<IMaybeNonVoidAntetype>().ToFixedList();
        var returnAntetype = function.ReturnType.ToAntetype();
        return new(function, null, parameterAntetypes, returnAntetype);
    }

    public static AntetypeContextualizedOverload<IConstructorDeclarationNode> Create(
        IMaybeAntetype constructingAntetype,
        IConstructorDeclarationNode constructor)
    {
        var symbol = constructor.Symbol;
        return Create(constructingAntetype, constructor, symbol.SelfParameterType);
    }

    public static AntetypeContextualizedOverload<IInitializerDeclarationNode> Create(
        IMaybeAntetype initializingAntetype, IInitializerDeclarationNode initializer)
    {
        var symbol = initializer.Symbol;
        return Create(initializingAntetype, initializer, symbol.SelfParameterType);
    }

    public static AntetypeContextualizedOverload<IStandardMethodDeclarationNode> Create(
        IMaybeExpressionAntetype contextAntetype,
        IStandardMethodDeclarationNode method)
    {
        var symbol = method.Symbol;
        return Create(contextAntetype, method, method.SelfParameterType.Type);
    }

    private static AntetypeContextualizedOverload<TDeclaration> Create<TDeclaration>(
        IMaybeExpressionAntetype contextAntetype,
        TDeclaration declaration,
        IMaybePseudotype selfParameterType)
        where TDeclaration : IInvocableDeclarationNode
    {
        var selfParameterAntetype = SelfParameterAntetype(contextAntetype, selfParameterType);
        var parameterAntetypes = ParameterAntetypes(contextAntetype, declaration);
        var returnAntetype = ReturnAntetype(contextAntetype, declaration);
        return new(declaration, selfParameterAntetype, parameterAntetypes, returnAntetype);
    }

    private static IMaybeAntetype SelfParameterAntetype(
        IMaybeExpressionAntetype contextAntetype,
        IMaybePseudotype symbolSelfParameterType)
        => contextAntetype.ReplaceTypeParametersIn(symbolSelfParameterType.ToAntetype())
                               .ToNonConstValueType();

    private static IFixedList<IMaybeNonVoidAntetype> ParameterAntetypes(
        IMaybeExpressionAntetype contextAntetype,
        IInvocableDeclarationNode declaration)
        => declaration.ParameterTypes.Select(p => ParameterAntetype(contextAntetype, p))
                      .OfType<IMaybeNonVoidAntetype>().ToFixedList();

    private static IMaybeAntetype ParameterAntetype(IMaybeExpressionAntetype contextAntetype, ParameterType parameter)
        => contextAntetype.ReplaceTypeParametersIn(parameter.Type.ToAntetype()).ToNonConstValueType();

    private static IMaybeAntetype ReturnAntetype(IMaybeExpressionAntetype contextAntetype, IInvocableDeclarationNode declaration)
        => contextAntetype.ReplaceTypeParametersIn(declaration.ReturnType.ToAntetype()).ToNonConstValueType();
}
