using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// A contextualized invokable symbol.
/// </summary>
/// <remarks>Contextualized symbols have type parameters replaced.</remarks>
public class Contextualized<TSymbol>
    where TSymbol : InvocableSymbol
{
    public TSymbol Symbol { get; }
    public SelfParameter? SelfParameterType { get; }
    public IFixedList<Parameter> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public Return ReturnType { get; }

    public Contextualized(
        TSymbol symbol,
        SelfParameter? effectiveSelfType,
        IFixedList<Parameter> parameterTypes,
        Return @return)
    {
        Symbol = symbol;
        SelfParameterType = effectiveSelfType;
        ParameterTypes = parameterTypes;
        ReturnType = @return;
    }
}
