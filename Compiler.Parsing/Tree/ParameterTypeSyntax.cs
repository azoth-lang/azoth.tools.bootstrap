using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ParameterTypeSyntax : CodeSyntax, IParameterTypeSyntax
{
    public bool IsLent { get; }
    public ITypeSyntax Referent { get; }

    public ParameterTypeSyntax(TextSpan span, bool isLent, ITypeSyntax referent)
        : base(span)
    {
        IsLent = isLent;
        Referent = referent;
    }

    public override string ToString() => $"{(IsLent ? "lent " : "")}{Referent}";
}
