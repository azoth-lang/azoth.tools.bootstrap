using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class MemberAccessExpressionSyntax : NameExpressionSyntax, IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public StandardName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public TextSpan MemberNameSpan { get; }

    public MemberAccessExpressionSyntax(
        TextSpan span,
        IExpressionSyntax context,
        IIdentifierNameExpressionSyntax member)
        : base(span)
    {
        Context = context;
        MemberName = member.Name!;
        TypeArguments = FixedList.Empty<ITypeSyntax>();
        MemberNameSpan = member.Span;
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
        => $"{Context.ToGroupedString(ExpressionPrecedence)}.{MemberName}";
}
