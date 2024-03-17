using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class LiteralExpression : Expression, ILiteralExpression
{
    protected LiteralExpression(TextSpan span, DataType dataType)
        : base(span, dataType) { }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
}
