using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(FunctionOrInitializerSymbol),
    typeof(MethodSymbol))]
public abstract class InvocableSymbol : Symbol
{
    public override PackageFacetSymbol? Facet => ContainingSymbol.Facet;
    public abstract override Symbol ContainingSymbol { get; }
    public abstract override IdentifierName? Name { get; }
    public IFixedList<ParameterType> ParameterTypes { get; }
    public virtual int Arity => ParameterTypes.Count;
    public abstract Type ReturnType { get; }

    private protected InvocableSymbol(IFixedList<ParameterType> parameterTypes)
    {
        ParameterTypes = parameterTypes;
    }
}
