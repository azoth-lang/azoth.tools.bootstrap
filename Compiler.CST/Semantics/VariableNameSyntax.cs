using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST.Semantics;

[Closed(typeof(SelfExpressionSyntax))]
public abstract class VariableNameSyntax : SyntaxSemantics
{
    public abstract BindingSymbol Symbol { get; }
    public override Promise<DataType> Type { get; } = new();

    protected VariableNameSyntax(Symbol symbol)
        : base(symbol) { }

    protected VariableNameSyntax(IFixedSet<Symbol> symbols)
        : base(symbols) { }
}
