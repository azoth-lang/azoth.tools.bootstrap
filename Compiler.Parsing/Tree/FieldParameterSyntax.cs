using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FieldParameterSyntax : ParameterSyntax, IFieldParameterSyntax
{
    public new IdentifierName Name { get; }
    public Promise<FieldSymbol?> ReferencedSymbol { get; } = new Promise<FieldSymbol?>();
    public override IPromise<DataType> DataType { get; }
    public IExpressionSyntax? DefaultValue { get; }

    public FieldParameterSyntax(TextSpan span, IdentifierName name, IExpressionSyntax? defaultValue)
        : base(span, name)
    {
        Name = name;
        DefaultValue = defaultValue;
        DataType = ReferencedSymbol.Select(s => s?.Type ?? Types.DataType.Unknown);
    }

    public override string ToString()
    {
        var defaultValue = DefaultValue is not null ? " = " + DefaultValue : "";
        return $".{Name}{defaultValue}";
    }
}
