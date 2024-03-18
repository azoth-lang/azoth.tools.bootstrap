using System.Diagnostics;
using System.Globalization;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BoolLiteralExpressionSyntax : LiteralExpressionSyntax, IBoolLiteralExpressionSyntax
{
    public bool Value { [DebuggerStepThrough] get; }
    public override Promise<BoolConstValueType> DataType { get; } = new Promise<BoolConstValueType>();

    public BoolLiteralExpressionSyntax(TextSpan span, bool value)
        : base(span)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
