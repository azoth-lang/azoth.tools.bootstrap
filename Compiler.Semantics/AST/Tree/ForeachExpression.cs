using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ForeachExpression : Expression, IForeachExpression
{
    public VariableSymbol Symbol { get; }
    BindingSymbol IBinding.Symbol => Symbol;
    NamedBindingSymbol ILocalBinding.Symbol => Symbol;
    public IExpression InExpression { get; }
    public IBlockExpression Block { get; }
    public Promise<bool> VariableIsLiveAfterAssignment { get; } = new Promise<bool>();

    public ForeachExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        VariableSymbol symbol,
        IExpression inExpression,
        IBlockExpression block)
        : base(span, dataType, semantics)
    {
        Symbol = symbol;
        InExpression = inExpression;
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
    {
        var binding = Symbol.IsMutableBinding ? "var " : "";
        var declarationNumber = Symbol.DeclarationNumber is null ? "" : "⟦#" + Symbol.DeclarationNumber + "⟧";
        return $"foreach {binding}{Symbol.Name}{declarationNumber}: {Symbol.DataType} in {InExpression} {Block}";
    }
}
