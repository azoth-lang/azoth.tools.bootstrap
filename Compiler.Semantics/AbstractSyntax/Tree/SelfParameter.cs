using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class SelfParameter : Parameter, ISelfParameter
{
    public SelfParameterSymbol Symbol { get; }
    BindingSymbol IBinding.Symbol => Symbol;
    public SelfParameter(TextSpan span, SelfParameterSymbol symbol, bool unused)
        : base(span, unused)
    {
        Symbol = symbol;
    }

    public override string ToString()
    {
        var value = "self";
        if (Symbol.IsMutableBinding) value = "mut " + value;
        return value;
    }
}
