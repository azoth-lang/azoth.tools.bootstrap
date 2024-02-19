using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BlockBodySyntax : Syntax, IBlockBodySyntax
{
    public IFixedList<IBodyStatementSyntax> Statements { [DebuggerStepThrough] get; }
    IFixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements => Statements;

    public BlockBodySyntax(TextSpan span, IFixedList<IBodyStatementSyntax> statements)
        : base(span)
    {
        Statements = statements;
    }

    public override string ToString() => "{ … }";
}
