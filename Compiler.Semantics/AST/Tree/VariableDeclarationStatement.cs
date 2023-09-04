using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class VariableDeclarationStatement : Statement, IVariableDeclarationStatement
{
    public TextSpan NameSpan { get; }
    public VariableSymbol Symbol { get; }
    BindingSymbol IBinding.Symbol => Symbol;
    NamedBindingSymbol ILocalBinding.Symbol => Symbol;
    public IExpression? Initializer { get; }
    public Promise<bool> VariableIsLiveAfter { get; } = new Promise<bool>();

    public VariableDeclarationStatement(
        TextSpan span,
        TextSpan nameSpan,
        VariableSymbol symbol,
        IExpression? initializer)
        : base(span)
    {
        NameSpan = nameSpan;
        Symbol = symbol;
        Initializer = initializer;
    }

    public override string ToString()
    {
        var binding = Symbol.IsMutableBinding ? "var" : "let";
        var declarationNumber = Symbol.DeclarationNumber is null ? "" : "⟦#" + Symbol.DeclarationNumber + "⟧";
        var initializer = Initializer != null ? " = " + Initializer : "";
        return $"{binding} {Symbol.Name}{declarationNumber}: {Symbol.DataType}{initializer};";
    }
}
