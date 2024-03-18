using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class MemberAccessExpressionSyntax : ExpressionSyntax, IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public AccessOperator AccessOperator { [DebuggerStepThrough] get; }
    public ISimpleNameExpressionSyntax Member { [DebuggerStepThrough] get; }
    public Promise<Symbol?> ReferencedSymbol => Member.ReferencedSymbol;

    public MemberAccessExpressionSyntax(
        TextSpan span,
        IExpressionSyntax context,
        AccessOperator accessOperator,
        ISimpleNameExpressionSyntax member)
        : base(span)
    {
        Context = context;
        AccessOperator = accessOperator;
        Member = member;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Context.ToGroupedString(ExpressionPrecedence)}{AccessOperator.ToSymbolString()}{Member}";
}
