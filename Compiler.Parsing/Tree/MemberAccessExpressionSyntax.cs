using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class MemberAccessExpressionSyntax : NameExpressionSyntax, IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public AccessOperator AccessOperator { [DebuggerStepThrough] get; }
    public IStandardNameExpressionSyntax Member { [DebuggerStepThrough] get; }
    public override Promise<DataType?> DataType { get; } = new();
    Promise<DataType> IDataTypedExpressionSyntax.DataType => DataType!;
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType!;
    public override Promise<Symbol?> ReferencedSymbol => Member.ReferencedSymbol;
    IPromise<Symbol?> INameExpressionSyntax.ReferencedSymbol => Member.ReferencedSymbol;
    IPromise<Symbol?> IAssignableExpressionSyntax.ReferencedSymbol => Member.ReferencedSymbol;

    public MemberAccessExpressionSyntax(
        TextSpan span,
        IExpressionSyntax context,
        AccessOperator accessOperator,
        IStandardNameExpressionSyntax member)
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
