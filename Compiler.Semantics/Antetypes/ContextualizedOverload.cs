using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
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
        var selfParameterAntetype = constructingAntetype.ReplaceTypeParametersIn(symbol.SelfParameterType.ToAntetype())
                                                        .ToNonConstValueType();

        var parameterAntetypes = symbol.Parameters.Select(p => ParameterAntetype(constructingAntetype, p))
                                       .OfType<IMaybeNonVoidAntetype>().ToFixedList();
        var returnAntetype = constructingAntetype.ReplaceTypeParametersIn(symbol.ReturnType.ToAntetype())
                                                 .ToNonConstValueType();
        return new(constructor, selfParameterAntetype, parameterAntetypes, returnAntetype);
    }

    private static IMaybeAntetype ParameterAntetype(IMaybeAntetype context, Parameter parameter)
        => context.ReplaceTypeParametersIn(parameter.Type.ToAntetype()).ToNonConstValueType();
}
