using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

[Closed(
    typeof(FunctionOrMethodSymbol),
    typeof(ConstructorSymbol))]
public abstract class InvocableSymbol : Symbol
{
    public override Symbol ContainingSymbol { get; }
    public override Name? Name { get; }
    public FixedList<DataType> ParameterDataTypes { get; }
    public int Arity => ParameterDataTypes.Count;
    public virtual DataType ReturnDataType { get; }

    protected InvocableSymbol(
        Symbol containingSymbol,
        Name? name,
        FixedList<DataType> parameterDataTypes,
        DataType returnDataType)
        : base(containingSymbol, name)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        ParameterDataTypes = parameterDataTypes;
        ReturnDataType = returnDataType;
    }
}
