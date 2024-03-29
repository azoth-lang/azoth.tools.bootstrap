using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ReturnTypeSyntax : Syntax, IReturnTypeSyntax
{
    public ITypeSyntax Referent { get; }

    public ReturnTypeSyntax(TextSpan span, ITypeSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    public override string ToString() => Referent.ToString();
}
