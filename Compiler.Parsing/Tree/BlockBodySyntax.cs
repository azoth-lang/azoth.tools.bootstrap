using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BlockBodySyntax : CodeSyntax, IBlockBodySyntax
{
    public IFixedList<IBodyStatementSyntax> Statements { [DebuggerStepThrough] get; }

    public BlockBodySyntax(TextSpan span, IFixedList<IBodyStatementSyntax> statements)
        : base(span)
    {
        Statements = statements;
    }

    public override string ToString() => "{ … }";
}
