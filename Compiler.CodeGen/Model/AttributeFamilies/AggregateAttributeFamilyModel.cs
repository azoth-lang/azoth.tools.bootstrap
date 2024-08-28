using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;

public sealed class AggregateAttributeFamilyModel : AttributeFamilyModel
{
    public AggregateAttributeFamilySyntax Syntax { get; }
    public override string Name => Syntax.Name;
    public override TypeModel Type { get; }
    public TypeModel FromType { get; }
    public string? ConstructExpression => Syntax.ConstructExpression;
    public string? AggregateMethod => Syntax.AggregateMethod;
    public string DoneMethod => Syntax.DoneMethod;

    public AggregateAttributeFamilyModel(TreeModel tree, AggregateAttributeFamilySyntax syntax)
        : base(tree)
    {
        Syntax = syntax;
        Type = TypeModel.CreateFromSyntax(tree, syntax.Type);
        FromType = TypeModel.CreateFromSyntax(tree, syntax.FromType);
    }

    public override string ToString()
    {
        var construct = ConstructExpression is not null ? $" => {ConstructExpression}" : "";
        var aggregate = AggregateMethod is not null ? $" with {AggregateMethod}" : "";
        return $"↗↖ *.{Name}: {Type} from {FromType}{construct}{aggregate} done {DoneMethod}";
    }
}
