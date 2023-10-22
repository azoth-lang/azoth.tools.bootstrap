using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class Body : AbstractSyntax, IBody
{
    public FixedList<IBodyStatement> Statements { get; }

    private readonly FixedList<IStatement> statements;
    FixedList<IStatement> IBodyOrBlock.Statements => statements;

    public Body(TextSpan span, FixedList<IBodyStatement> statements)
        : base(span)
    {
        Statements = statements;
        this.statements = statements.ToFixedList<IStatement>();
    }

    public override string ToString() => "{…}";
}
