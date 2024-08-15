using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class PatternMatchExpressionSyntax : ExpressionSyntax, IPatternMatchExpressionSyntax
{
    public IExpressionSyntax Referent { get; }
    public IPatternSyntax Pattern { get; }

    public PatternMatchExpressionSyntax(IExpressionSyntax referent, IPatternSyntax pattern)
        : base(TextSpan.Covering(referent.Span, pattern.Span))
    {
        Referent = referent;
        Pattern = pattern;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Conversion;

    public override string ToString() =>
        $"{Referent.ToGroupedString(ExpressionPrecedence)} is {Pattern}";
}
