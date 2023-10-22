using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// A contextualized invokable symbol
/// </summary>
/// <remarks>Contextualized symbols have type parameters replaced.</remarks>
public class Contextualized<TSymbol>
    where TSymbol : InvocableSymbol
{
    public TSymbol Symbol { get; }
    public FixedList<ParameterType> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public ReturnType ReturnType { get; }

    public Contextualized(
        TSymbol symbol,
        FixedList<ParameterType> parameterTypes,
        ReturnType returnType)
    {
        Symbol = symbol;
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }
}
