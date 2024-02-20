using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(FunctionOrMethodSymbol),
    typeof(ConstructorSymbol))]
public abstract class InvocableSymbol : Symbol
{
    public override PackageSymbol? Package { get; }
    public override Symbol ContainingSymbol { get; }
    public override SimpleName? Name { get; }
    public IFixedList<Parameter> Parameters { get; }
    public int Arity => Parameters.Count;
    public Return Return { get; }

    protected InvocableSymbol(
        Symbol containingSymbol,
        SimpleName? name,
        IFixedList<Parameter> parameters,
        Return @return)
        : base(name)
    {
        Package = containingSymbol.Package;
        ContainingSymbol = containingSymbol;
        Name = name;
        Parameters = parameters;
        Return = @return;
    }
}
