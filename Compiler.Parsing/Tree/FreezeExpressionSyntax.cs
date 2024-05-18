using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FreezeExpressionSyntax : DataTypedExpressionSyntax, IFreezeExpressionSyntax
{
    public ISimpleNameSyntax Referent { [DebuggerStepThrough] get; }

    public Promise<BindingSymbol?> ReferencedSymbol { [DebuggerStepThrough] get; }
        = new Promise<BindingSymbol?>();

    public FreezeExpressionSyntax(TextSpan span, ISimpleNameSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"freeze {Referent}";
}
