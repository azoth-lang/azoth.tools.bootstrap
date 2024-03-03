using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(FunctionOrInitializerSymbol),
    typeof(MethodSymbol),
    typeof(ConstructorSymbol))]
public abstract class InvocableSymbol : Symbol
{
    public override PackageSymbol? Package => ContainingSymbol.Package;
    public abstract override Symbol ContainingSymbol { get; }
    public abstract override SimpleName? Name { get; }
    public IFixedList<Parameter> Parameters { get; }
    public int Arity => Parameters.Count;
    public Return Return { get; }

    protected InvocableSymbol(
        IFixedList<Parameter> parameters,
        Return @return)
    {
        Parameters = parameters;
        Return = @return;
    }
}
