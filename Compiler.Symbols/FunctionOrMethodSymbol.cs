using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(FunctionSymbol),
    typeof(MethodSymbol))]
public abstract class FunctionOrMethodSymbol : InvocableSymbol
{
    public override SimpleName Name { get; }

    protected FunctionOrMethodSymbol(
        Symbol containingSymbol,
        SimpleName name,
        IFixedList<Parameter> parameters,
        ReturnType returnType)
        : base(containingSymbol, name, parameters, returnType)
    {
        Name = name;
    }
}
