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
    public SelfParameterType? SelfParameterType { get; }
    public IFixedList<ParameterType> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public ReturnType ReturnType { get; }

    public Contextualized(
        TSymbol symbol,
        SelfParameterType? effectiveSelfType,
        IFixedList<ParameterType> parameterTypes,
        ReturnType returnType)
    {
        Symbol = symbol;
        SelfParameterType = effectiveSelfType;
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }
}
