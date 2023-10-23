using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class PatternMatchExpression : Expression, IPatternMatchExpression
{
    public IExpression Referent { get; }
    public IPattern Pattern { get; }

    public PatternMatchExpression(IExpression referent, IPattern pattern)
        : base(TextSpan.Covering(referent.Span, pattern.Span), DataType.Bool, ExpressionSemantics.CopyValue)
    {
        Referent = referent;
        Pattern = pattern;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Relational;

    public override string ToString()
        => $"{Referent.ToGroupedString(ExpressionPrecedence)} is {Pattern}";
}
