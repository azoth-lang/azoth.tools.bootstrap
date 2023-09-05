using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ForeachExpressionSyntax : ExpressionSyntax, IForeachExpressionSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public Name VariableName { [DebuggerStepThrough] get; }
    public Promise<int?> DeclarationNumber { [DebuggerStepThrough] get; } = new Promise<int?>();
    public Promise<VariableSymbol> Symbol { [DebuggerStepThrough] get; } = new Promise<VariableSymbol>();
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;

    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax InExpression { [DebuggerStepThrough] get; }

    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }

    public ForeachExpressionSyntax(
        TextSpan span,
        bool isMutableBinding,
        Name variableName,
        ITypeSyntax? typeSyntax,
        IExpressionSyntax inExpression,
        IBlockExpressionSyntax block)
        : base(span)
    {
        IsMutableBinding = isMutableBinding;
        VariableName = variableName;
        InExpression = inExpression;
        Block = block;
        Type = typeSyntax;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
    {
        var binding = IsMutableBinding ? "var " : "";
        return $"foreach {binding}{VariableName}: {Type} in {InExpression} {Block}";
    }
}
