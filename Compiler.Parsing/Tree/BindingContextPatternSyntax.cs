using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BindingContextPatternSyntax : Syntax, IBindingContextPatternSyntax
{
    public bool IsMutableBinding { get; }
    public IPatternSyntax Pattern { get; }
    public ITypeSyntax? Type { get; }

    public BindingContextPatternSyntax(TextSpan span, bool isMutableBinding, IPatternSyntax pattern, ITypeSyntax? type)
        : base(span)
    {
        IsMutableBinding = isMutableBinding;
        Pattern = pattern;
        Type = type;
    }

    public override string ToString()
    {
        var binding = IsMutableBinding ? "var" : "let";
        var type = Type is not null ? ": " + Type : "";
        return $"{binding} {Pattern}{type}";
    }
}
