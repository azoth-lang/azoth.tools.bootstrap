using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ForeachExpressionSyntax : ExpressionSyntax, IForeachExpressionSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { get; }
    public IdentifierName VariableName { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax InExpression { [DebuggerStepThrough] get; }

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

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
    {
        var binding = IsMutableBinding ? "var " : "";
        var type = Type is not null ? $": {Type} " : "";
        return $"foreach {binding}{VariableName}{type} in {InExpression} {Block}";
    }
}
