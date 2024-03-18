using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class BindingPattern : Pattern, IBindingPattern
{
    public NamedVariableSymbol Symbol { get; }
    NamedBindingSymbol ILocalBinding.Symbol => Symbol;
    BindingSymbol IBinding.Symbol => Symbol;

    public Promise<bool> VariableIsLiveAfter { get; } = new Promise<bool>();

    public BindingPattern(TextSpan span, NamedVariableSymbol symbol)
        : base(span)
    {
        Symbol = symbol;
    }

    public override string ToString()
    {
        var declarationNumber = Symbol.DeclarationNumber is null ? "" : "⟦#" + Symbol.DeclarationNumber + "⟧";
        return $"{Symbol.Name}{declarationNumber}";
    }
}
