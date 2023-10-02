using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ReturnSyntax : Syntax, IReturnSyntax
{
    public bool IsLent { get; }
    public ITypeSyntax Type { get; }

    public ReturnSyntax(TextSpan span, bool isLent, ITypeSyntax type)
        : base(span)
    {
        IsLent = isLent;
        Type = type;
    }

    public override string ToString() => IsLent ? $"-> lent {Type}" : $"-> {Type}";
}
