using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class NamedParameter : Parameter, INamedParameter
{
    public VariableSymbol Symbol { get; }
    BindingSymbol IBinding.Symbol => Symbol;
    NamedBindingSymbol ILocalBinding.Symbol => Symbol;
    public IExpression? DefaultValue { get; }

    public NamedParameter(
        TextSpan span,
        VariableSymbol symbol,
        bool unused,
        IExpression? defaultValue)
        : base(span, unused)
    {
        Symbol = symbol;
        DefaultValue = defaultValue;
    }

    public override string ToString()
    {
        var mutable = Symbol.IsMutableBinding ? "var " : "";
        var defaultValue = DefaultValue is not null ? " = " + DefaultValue : "";
        var declarationNumber = Symbol.DeclarationNumber is null ? "" : "⟦#" + Symbol.DeclarationNumber + "⟧";
        return $"{mutable}{Symbol.Name}{declarationNumber}: {Symbol.Type.ToILString()}{defaultValue}";
    }
}
