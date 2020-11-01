using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class Body : AbstractSyntax, IBody
    {
        public FixedList<IBodyStatement> Statements { get; }
        FixedList<IStatement> IBodyOrBlock.Statements => Statements.ToFixedList<IStatement>();

        public Body(TextSpan span, FixedList<IBodyStatement> statements)
            : base(span)
        {
            Statements = statements;
        }

        public override string ToString()
        {
            return "{ … }";
        }
    }
}
