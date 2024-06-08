using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal sealed class ContextualizedOverload<TDeclaration>
    where TDeclaration : IInvocableDeclarationNode
{
    public TDeclaration Declaration { get; }
    public IMaybeAntetype? SelfParameterAntetype { get; }
    public IFixedList<IMaybeNonVoidAntetype> ParameterAntetypes { get; }
    public int Arity => ParameterAntetypes.Count;
    public IMaybeAntetype ReturnAntetype { get; }

    internal ContextualizedOverload(
        TDeclaration declaration,
        IMaybeAntetype selfParameterAntetype,
        IEnumerable<IMaybeNonVoidAntetype> parameterAntetypes,
        IMaybeAntetype returnAntetype)
    {
        SelfParameterAntetype = selfParameterAntetype;
        ParameterAntetypes = parameterAntetypes.ToFixedList();
        ReturnAntetype = returnAntetype;
        Declaration = declaration;
    }
}

internal static class ContextualizedOverload
{
    public static ContextualizedOverload<IConstructorDeclarationNode> Create(
        IMaybeAntetype constructingAntetype,
        IConstructorDeclarationNode constructor)
    {
        var symbol = constructor.Symbol;
        return Create(constructingAntetype, constructor, symbol, symbol.SelfParameterType);
    }

    public static ContextualizedOverload<IInitializerDeclarationNode> Create(
        IMaybeAntetype initializingAntetype, IInitializerDeclarationNode initializer)
    {
        var symbol = initializer.Symbol;
        return Create(initializingAntetype, initializer, symbol, symbol.SelfParameterType);
    }

    private static ContextualizedOverload<TDeclaration> Create<TDeclaration>(
        IMaybeAntetype contextAntetype,
        TDeclaration declaration,
        InvocableSymbol symbol,
        CapabilityType selfParameterType)
        where TDeclaration : IInvocableDeclarationNode
    {
        var selfParameterAntetype = SelfParameterAntetype(contextAntetype, selfParameterType);
        var parameterAntetypes = ParameterAntetypes(contextAntetype, symbol);
        var returnAntetype = ReturnAntetype(contextAntetype, symbol);
        return new(declaration, selfParameterAntetype, parameterAntetypes, returnAntetype);
    }

    private static IMaybeAntetype SelfParameterAntetype(
        IMaybeAntetype contextAntetype,
        CapabilityType symbolSelfParameterType)
        => contextAntetype.ReplaceTypeParametersIn(symbolSelfParameterType.ToAntetype())
                               .ToNonConstValueType();

    private static IFixedList<IMaybeNonVoidAntetype> ParameterAntetypes(
        IMaybeAntetype contextAntetype,
        InvocableSymbol symbol)
        => symbol.Parameters.Select(p => ParameterAntetype(contextAntetype, p))
                 .OfType<IMaybeNonVoidAntetype>().ToFixedList();

    private static IMaybeAntetype ParameterAntetype(IMaybeAntetype context, Parameter parameter)
        => context.ReplaceTypeParametersIn(parameter.Type.ToAntetype()).ToNonConstValueType();

    private static IMaybeAntetype ReturnAntetype(IMaybeAntetype contextAntetype, InvocableSymbol symbol)
        => contextAntetype.ReplaceTypeParametersIn(symbol.Return.Type.ToAntetype()).ToNonConstValueType();
}
