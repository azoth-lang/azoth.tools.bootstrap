using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FieldParameterSyntax : ParameterSyntax, IFieldParameterSyntax
{
    public new IdentifierName Name { get; }
    public override IPromise<DataType> DataType { get; }
    public IExpressionSyntax? DefaultValue { get; }

    public FieldParameterSyntax(TextSpan span, IdentifierName name, IExpressionSyntax? defaultValue)
        : base(span, name)
    {
        Name = name;
        DefaultValue = defaultValue;
        DataType = Types.DataType.PromiseOfUnknown;
    }

    public override string ToString()
    {
        var defaultValue = DefaultValue is not null ? " = " + DefaultValue : "";
        return $".{Name}{defaultValue}";
    }
}
