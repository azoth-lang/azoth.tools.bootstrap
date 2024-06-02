using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class RecoverConstExpression : RecoverExpression, IRecoverConstExpression
{
    public RecoverConstExpression(TextSpan span, CapabilityType dataType, IExpression value)
        : base(span, dataType, value) { }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"⟦recover_const⟧ {Value}";
}
