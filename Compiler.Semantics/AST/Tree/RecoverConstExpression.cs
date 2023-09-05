using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class RecoverConstExpression : RecoverExpression, IRecoverConstExpression
{
    public RecoverConstExpression(TextSpan span, ReferenceType dataType, ExpressionSemantics semantics, IExpression value)
        : base(span, dataType, semantics, value) { }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"⟦recover_const⟧ {Value}";
}
