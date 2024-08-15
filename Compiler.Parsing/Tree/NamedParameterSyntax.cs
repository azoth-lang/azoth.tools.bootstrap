using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NamedParameterSyntax : ParameterSyntax, INamedParameterSyntax
{
    public bool IsMutableBinding { get; }
    public bool IsLentBinding { get; }
    public TextSpan NameSpan { get; }
    public new IdentifierName Name { get; }
    public ITypeSyntax Type { get; }
    public IExpressionSyntax? DefaultValue { get; }

    public NamedParameterSyntax(
        TextSpan span,
        bool isMutableBinding,
        bool isLentBinding,
        TextSpan nameSpan,
        IdentifierName name,
        ITypeSyntax typeSyntax,
        IExpressionSyntax? defaultValue)
        : base(span, name)
    {
        IsMutableBinding = isMutableBinding;
        Name = name;
        Type = typeSyntax;
        DefaultValue = defaultValue;
        IsLentBinding = isLentBinding;
        NameSpan = nameSpan;
    }

    public override string ToString()
    {
        var lent = IsLentBinding ? "lent " : "";
        var mutable = IsMutableBinding ? "var " : "";
        var defaultValue = DefaultValue is not null ? " = " + DefaultValue : "";
        return $"{lent}{mutable}{Name}: {Type}{defaultValue}";
    }
}
