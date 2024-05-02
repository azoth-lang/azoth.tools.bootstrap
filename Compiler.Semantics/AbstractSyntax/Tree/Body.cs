using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class Body : AbstractSyntax, IBody
{
    public IFixedList<IBodyStatement> Statements { get; }
    IFixedList<IStatement> IBodyOrBlock.Statements => Statements;

    public Body(TextSpan span, IFixedList<IBodyStatement> statements)
        : base(span)
    {
        Statements = statements;
    }

    public override string ToString() => "{…}";
}
