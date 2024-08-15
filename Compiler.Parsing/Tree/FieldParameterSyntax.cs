using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FieldParameterSyntax : ParameterSyntax, IFieldParameterSyntax
{
    public new IdentifierName Name { get; }
    public IExpressionSyntax? DefaultValue { get; }

    public FieldParameterSyntax(TextSpan span, IdentifierName name, IExpressionSyntax? defaultValue)
        : base(span, name)
    {
        Name = name;
        DefaultValue = defaultValue;
    }

    public override string ToString()
    {
        var defaultValue = DefaultValue is not null ? " = " + DefaultValue : "";
        return $".{Name}{defaultValue}";
    }
}
