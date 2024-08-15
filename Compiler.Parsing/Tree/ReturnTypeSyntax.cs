using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ReturnTypeSyntax : CodeSyntax, IReturnTypeSyntax
{
    public ITypeSyntax Referent { get; }

    public ReturnTypeSyntax(TextSpan span, ITypeSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    public override string ToString() => Referent.ToString();
}
