using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(FunctionSymbol),
    typeof(MethodSymbol))]
public abstract class FunctionOrMethodSymbol : InvocableSymbol
{
    public override Name Name { get; }

    protected FunctionOrMethodSymbol(
        Symbol containingSymbol,
        Name name,
        FixedList<DataType> parameterDataTypes,
        DataType returnDataType)
        : base(containingSymbol, name, parameterDataTypes, returnDataType)
    {
        Name = name;
    }
}
