using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.AttributeFamilies;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;

public sealed class AggregateAttributeFamilyModel : AttributeFamilyModel
{
    public AggregateAttributeFamilySyntax Syntax { get; }
    public override string Name => Syntax.Name;
    public override TypeModel Type { get; }
    public TypeModel FromType { get; }
    public string ConstructExpression { get; }
    public string AggregateMethod { get; }
    public string DoneMethod => Syntax.DoneMethod;

    public AggregateAttributeFamilyModel(TreeModel tree, AggregateAttributeFamilySyntax syntax)
        : base(tree)
    {
        Syntax = syntax;
        Type = TypeModel.CreateFromSyntax(tree, syntax.Type);
        FromType = TypeModel.CreateFromSyntax(tree, syntax.FromType);
        ConstructExpression = Syntax.ConstructExpression ?? $"new {Emit.Type(FromType)}()";
        AggregateMethod = Syntax.AggregateMethod ?? "Add";
    }

    public override string ToString()
        => $"↗↖ *.{Name}: {Type} from {FromType} => {ConstructExpression} with {AggregateMethod} done {DoneMethod}";
}
