using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class MemberAccessExpressionSyntax : NameExpressionSyntax, IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public AccessOperator AccessOperator { [DebuggerStepThrough] get; }
    public StandardName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public TextSpan MemberNameSpan { get; }
    public override Promise<IMemberAccessSyntaxSemantics> Semantics { [DebuggerStepThrough] get; } = new();
    IPromise<ISyntaxSemantics> INameExpressionSyntax.Semantics => Semantics;
    public override IPromise<DataType?> DataType { [DebuggerStepThrough] get; }
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType!;
    public override IPromise<Symbol?> ReferencedSymbol { [DebuggerStepThrough] get; }

    public MemberAccessExpressionSyntax(
        TextSpan span,
        IExpressionSyntax context,
        AccessOperator accessOperator,
        IIdentifierNameExpressionSyntax member)
        : base(span)
    {
        Context = context;
        AccessOperator = accessOperator;
        MemberName = member.Name!;
        TypeArguments = FixedList.Empty<ITypeSyntax>();
        DataType = Semantics.Select(s => s.Type).Flatten();
        ReferencedSymbol = Semantics.Select(s => s.Symbol).Flatten();
        MemberNameSpan = member.Span;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Context.ToGroupedString(ExpressionPrecedence)}{AccessOperator.ToSymbolString()}{MemberName}";
}
