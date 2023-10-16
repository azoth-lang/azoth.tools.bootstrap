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
    public override SimpleName? Name { get; }
    public FixedList<ParameterType> ParameterTypes { get; }
    public int Arity => ParameterTypes.Count;
    public ReturnType ReturnType { get; }

    protected InvocableSymbol(
        Symbol containingSymbol,
        SimpleName? name,
        FixedList<ParameterType> parameterTypes,
        ReturnType returnType)
        : base(containingSymbol, name)
    {
        ContainingSymbol = containingSymbol;
        Name = name;
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
    }
}
