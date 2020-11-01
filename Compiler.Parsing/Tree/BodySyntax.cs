using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class BodySyntax : Syntax, IBodySyntax
    {
        public FixedList<IBodyStatementSyntax> Statements { [DebuggerStepThrough] get; }
        private readonly FixedList<IStatementSyntax> statements;
        FixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements
        {
            [DebuggerStepThrough]
            get => statements;
        }

        public BodySyntax(TextSpan span, FixedList<IBodyStatementSyntax> statements)
            : base(span)
        {
            Statements = statements;
            this.statements = statements.ToFixedList<IStatementSyntax>();
        }

        public override string ToString()
        {
            return "{ … }";
        }
    }
}
