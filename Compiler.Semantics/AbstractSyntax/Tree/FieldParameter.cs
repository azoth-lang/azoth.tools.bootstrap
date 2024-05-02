using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class FieldParameter : Parameter, IFieldParameter
{
    public FieldSymbol ReferencedSymbol { get; }
    public IExpression? DefaultValue { get; }

    public FieldParameter(
        TextSpan span,
        FieldSymbol referencedSymbol,
        IExpression? defaultValue)
        : base(span, false)
    {
        ReferencedSymbol = referencedSymbol;
        DefaultValue = defaultValue;
    }

    public override string ToString()
    {
        var defaultValue = DefaultValue is not null ? " = " + DefaultValue : "";
        return $".{ReferencedSymbol.Name}{defaultValue}";
    }
}
