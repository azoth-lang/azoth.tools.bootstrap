using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class NameExpressionSyntax : ExpressionSyntax, INameExpressionSyntax
{
    public abstract IPromise<ISyntaxSemantics> Semantics { get; }
    public abstract IPromise<Symbol?> ReferencedSymbol { get; }

    protected NameExpressionSyntax(TextSpan span)
        : base(span) { }
}
