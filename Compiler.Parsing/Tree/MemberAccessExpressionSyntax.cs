using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class MemberAccessExpressionSyntax : NameExpressionSyntax, IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public StandardName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public TextSpan MemberNameSpan { get; }
    public override Promise<IMemberAccessSyntaxSemantics> Semantics { [DebuggerStepThrough] get; } = new();
    public override IPromise<DataType?> DataType { [DebuggerStepThrough] get; }
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType!;

    public MemberAccessExpressionSyntax(
        TextSpan span,
        IExpressionSyntax context,
        IIdentifierNameExpressionSyntax member)
        : base(span)
    {
        Context = context;
        MemberName = member.Name!;
        TypeArguments = FixedList.Empty<ITypeSyntax>();
        DataType = Semantics.Select(s => s.Type).Flatten();
        MemberNameSpan = member.Span;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Context.ToGroupedString(ExpressionPrecedence)}.{MemberName}";
}
