using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

[SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
internal class MoveExpressionSyntax : ExpressionSyntax, IMoveExpressionSyntax
{
    public IVariableNameExpressionSyntax Referent { [DebuggerStepThrough] get; }

    public Promise<BindingSymbol?> ReferencedSymbol { [DebuggerStepThrough] get; }
        = new Promise<BindingSymbol?>();

    public MoveExpressionSyntax(TextSpan span, IVariableNameExpressionSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"move {Referent}";
}
