using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class MutateExpressionSyntax : ExpressionSyntax, IMutateExpressionSyntax
{
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public Promise<BindingSymbol?> ReferencedSymbol { [DebuggerStepThrough] get; } = new Promise<BindingSymbol?>();

    public MutateExpressionSyntax(TextSpan span, IExpressionSyntax referent)
        : base(span, ExpressionSemantics.MutableReference)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"mut {Referent}";
}
