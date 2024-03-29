using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class Expression : AbstractSyntax, IExpression
{
    public DataType DataType { [DebuggerStepThrough] get; }

    protected Expression(TextSpan span, DataType dataType)
        : base(span)
    {
        DataType = dataType;
    }

    protected abstract OperatorPrecedence ExpressionPrecedence { get; }

    public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        => surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
}
