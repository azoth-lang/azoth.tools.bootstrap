using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ForeachExpressionSyntax : DataTypedExpressionSyntax, IForeachExpressionSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { get; }
    public IdentifierName VariableName { [DebuggerStepThrough] get; }
    public Promise<int?> DeclarationNumber { [DebuggerStepThrough] get; } = new Promise<int?>();

    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax InExpression { [DebuggerStepThrough] get; }
    public Promise<MethodSymbol?> IterateMethod { get; } = new Promise<MethodSymbol?>();
    public Promise<MethodSymbol> NextMethod { get; } = new Promise<MethodSymbol>();

    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }

    public ForeachExpressionSyntax(
        TextSpan span,
        bool isMutableBinding,
        TextSpan nameSpan,
        IdentifierName variableName,
        ITypeSyntax? typeSyntax,
        IExpressionSyntax inExpression,
        IBlockExpressionSyntax block)
        : base(span)
    {
        IsMutableBinding = isMutableBinding;
        NameSpan = nameSpan;
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
