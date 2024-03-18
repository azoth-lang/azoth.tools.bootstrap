using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ForeachExpressionSyntax : ExpressionSyntax, IForeachExpressionSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName VariableName { [DebuggerStepThrough] get; }
    public Promise<int?> DeclarationNumber { [DebuggerStepThrough] get; } = new Promise<int?>();
    public Promise<NamedVariableSymbol> Symbol { [DebuggerStepThrough] get; } = new Promise<NamedVariableSymbol>();
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;

    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax InExpression { [DebuggerStepThrough] get; }
    public Promise<MethodSymbol?> IterateMethod { get; } = new Promise<MethodSymbol?>();
    public Promise<MethodSymbol> NextMethod { get; } = new Promise<MethodSymbol>();

    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }

    public ForeachExpressionSyntax(
        TextSpan span,
        bool isMutableBinding,
        IdentifierName variableName,
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
        var type = Type is not null ? $": {Type} " : "";
        return $"foreach {binding}{VariableName}{type} in {InExpression} {Block}";
    }
}
