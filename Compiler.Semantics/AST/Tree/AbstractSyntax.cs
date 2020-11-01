using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    internal abstract class AbstractSyntax : IAbstractSyntax
    {
        public TextSpan Span { get; }

        protected AbstractSyntax(TextSpan span)
        {
            Span = span;
        }

        public abstract override string ToString();
    }
}
