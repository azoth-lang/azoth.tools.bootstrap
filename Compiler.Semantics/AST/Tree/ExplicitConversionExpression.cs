using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class ExplicitConversionExpression : Expression, IExplicitConversionExpression
{
    public IExpression Expression { get; }
    public bool IsOptional { get; }

    protected ExplicitConversionExpression(
        TextSpan span, DataType dataType, IExpression expression, bool isOptional)
        : base(span, dataType)
    {
        Expression = expression;
        IsOptional = isOptional;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Conversion;
}
