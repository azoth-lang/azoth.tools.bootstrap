using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

public sealed class AggregateAttributeFamilySyntax : AttributeFamilySyntax
{
    public TypeSyntax FromType { get; }
    public string? ConstructExpression { get; }
    public string? AggregateMethod { get; }
    public string DoneMethod { get; }

    public AggregateAttributeFamilySyntax(
        string name,
        TypeSyntax type,
        TypeSyntax fromType,
        string? constructExpression,
        string? aggregateMethod,
        string doneMethod)
        : base(name, type)
    {
        FromType = fromType;
        ConstructExpression = constructExpression;
        AggregateMethod = aggregateMethod;
        DoneMethod = doneMethod;
    }

    public override string ToString()
    {
        var construct = ConstructExpression is not null ? $" => {ConstructExpression}" : "";
        var aggregate = AggregateMethod is not null ? $" with {AggregateMethod}" : "";
        return $"↗↖ *.{Name}: {Type} from {FromType}{construct}{aggregate} done {DoneMethod}";
    }
}
