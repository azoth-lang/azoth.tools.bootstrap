using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal abstract class Statement : AbstractSyntax, IStatement
    {
        protected Statement(TextSpan span)
            : base(span) { }
    }
}
