using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal sealed class ReturnSyntax : Syntax, IReturnSyntax
{
    public ITypeSyntax Type { get; }

    public ReturnSyntax(TextSpan span, ITypeSyntax type)
        : base(span)
    {
        Type = type;
    }

    public override string ToString() => $"-> {Type}";
}
