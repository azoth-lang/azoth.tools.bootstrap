using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(typeof(INamedVariableSymbol), typeof(ISelfParameterSymbol))]
public interface IVariableSymbol : IBindingSymbol
{
    public new InvocableSymbol ContainingSymbol { get; }
}
